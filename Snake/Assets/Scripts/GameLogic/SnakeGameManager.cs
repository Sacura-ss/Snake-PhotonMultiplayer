using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DefaultNamespace.Saving;
using DisplayScripts.Services;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using UnityEngine.Serialization;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameLogic
{
    public class SnakeGameManager: MonoBehaviourPunCallbacks
    {
        public static SnakeGameManager Instance = null;

        public Text InfoText;
        [SerializeField] private Spawner _spawner;

        [Header("Objects for game")] 
        [SerializeField] private List<SpawnObject> _spawnPrefabs;
        [SerializeField] private GameObject _snakePrefab;
     

        #region UNITY

        public void Awake()
        {
            Instance = this;
        }

        public override void OnEnable()
        {
            base.OnEnable();

            CountdownTimer.OnCountdownTimerHasExpired += OnCountdownTimerIsExpired;
        }

        public void Start()
        {
            Hashtable props = new Hashtable
            {
                {SnakeGame.PLAYER_LOADED_LEVEL, true}
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);
        }

        public override void OnDisable()
        {
            base.OnDisable();

            CountdownTimer.OnCountdownTimerHasExpired -= OnCountdownTimerIsExpired;
        }

        #endregion

        #region COROUTINES

        private IEnumerator SpawnFood()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(SnakeGame.FOOD_MIN_SPAWN_TIME, SnakeGame.FOOD_MAX_SPAWN_TIME));

                _spawner.SpawnInScreen(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Count)]);
            }
        }

        private IEnumerator EndOfGame(string winner, int score)
        {
            float timer = 3.0f;

            while (timer > 0.0f)
            {
                InfoText.text = string.Format("Player {0} won with {1} points.\n\n\nGo to finish screen in {2} seconds.", winner, score, timer.ToString("n1"));

                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        #endregion

        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("FinishMenu");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartCoroutine(SpawnFood());
            }
        }

        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            CheckEndOfGame();
        }

        public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
        {
            if (changedProps.ContainsKey(SnakeGame.PLAYER_LIVES))
            {
                CheckEndOfGame();
                return;
            }

            if (!PhotonNetwork.IsMasterClient)
            {
                return;
            }


            // if there was no countdown yet, the master client (this one) waits until everyone loaded the level and sets a timer start
            int startTimestamp;
            bool startTimeIsSet = CountdownTimer.TryGetStartTime(out startTimestamp);

            if (changedProps.ContainsKey(SnakeGame.PLAYER_LOADED_LEVEL))
            {
                if (CheckAllPlayerLoadedLevel())
                {
                    if (!startTimeIsSet)
                    {
                        CountdownTimer.SetStartTime();
                    }
                }
                else
                {
                    // not all players loaded yet. wait:
                    Debug.Log("setting text waiting for players! ",this.InfoText);
                    InfoText.text = "Waiting for other players...";
                }
            }
        
        }

        #endregion

        
        // called by OnCountdownTimerIsExpired() when the timer ended
        private void StartGame()
        {
            Debug.Log("StartGame!");

            // on rejoin, we have to figure out if the spaceship exists or not
            // if this is a rejoin (the ship is already network instantiated and will be setup via event) we don't need to call PN.Instantiate

            
            float angularStart = (360.0f / PhotonNetwork.CurrentRoom.PlayerCount) * PhotonNetwork.LocalPlayer.GetPlayerNumber();
            float x = 1.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            float y = 1.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            Vector3 position = new Vector3(x, y, 0.0f);
            Quaternion rotation = Quaternion.Euler(0.0f, 0.0f, angularStart);

            PhotonNetwork.Instantiate(_snakePrefab.name, position, rotation, 0);      // avoid this call on rejoin (ship was network instantiated before)

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnFood());
            }
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(SnakeGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    Debug.Log("VAR " + playerLoadedLevel);
                    if ((bool) playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        private void CheckEndOfGame()
        {
            bool allDestroyed = true;

            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object lives;
                if (p.CustomProperties.TryGetValue(SnakeGame.PLAYER_LIVES, out lives))
                {
                    if ((int)lives > 0)
                    {
                        allDestroyed = false;
                        break;
                    }
                }
            }

            if (allDestroyed)
            {
                if (PhotonNetwork.IsMasterClient)
                {
                    StopAllCoroutines();
                }

                string winner = "";
                int score = -1;

                foreach (Player p in PhotonNetwork.PlayerList)
                {
                    if (p.GetScore() > score)
                    {
                        winner = p.NickName;
                        score = p.GetScore();
                    }
                }
                
                SavedData.WinnerName = winner;
                SavedData.WinnerScore = score.ToString();

                StartCoroutine(EndOfGame(winner, score));
            }
        }

        private void OnCountdownTimerIsExpired()
        {
            StartGame();
        }
    }
    
    
}