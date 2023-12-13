using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using DisplayScripts.Services;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using Photon.Pun.UtilityScripts;
using Hashtable = ExitGames.Client.Photon.Hashtable;

namespace GameLogic
{
    public class SnakeGameLogic : MonoBehaviourPunCallbacks
    {
        [Header("Objects for game")] [SerializeField]
        private List<SpawnObject> _spawnPrefabs;

        [SerializeField] private GameObject _snakePrefab;

        [Header("Speed Settings")] [SerializeField]
        private float _spawnSpeedInSeconds = 1f;

        [SerializeField] private float SpeedUpValue = 0.3f;
        [SerializeField] private float MinSpeed = 1f;

        [SerializeField] private Spawner _spawner;

        #region UNITY

        public void Start()
        {
            Hashtable props = new Hashtable
            {
                { SnakeGame.PLAYER_LOADED_LEVEL, true }
            };
            PhotonNetwork.LocalPlayer.SetCustomProperties(props);

            float angularStart = (360.0f / PhotonNetwork.CurrentRoom.PlayerCount) *
                                 PhotonNetwork.LocalPlayer.GetPlayerNumber();
            float x = 20.0f * Mathf.Sin(angularStart * Mathf.Deg2Rad);
            float z = 20.0f * Mathf.Cos(angularStart * Mathf.Deg2Rad);
            Vector3 position = new Vector3(x, 0.0f, z);
            Quaternion rotation = Quaternion.Euler(0.0f, angularStart, 0.0f);

            if (PhotonNetwork.IsMasterClient)
            {
                StartCoroutine(SpawnCountObjectsByTime());
            }

            PhotonNetwork.Instantiate(_snakePrefab.name, position, rotation, 0);
        }

        #endregion


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

                StartCoroutine(EndOfGame(winner, score));
            }
        }

        #region COROUTINES

        private IEnumerator EndOfGame(string winner, int score)
        {
            float timer = 3.0f;

            while (timer > 0.0f)
            {
                yield return new WaitForEndOfFrame();

                timer -= Time.deltaTime;
            }

            PhotonNetwork.LeaveRoom();
        }

        private IEnumerator SpawnCountObjectsByTime()
        {
            while (true)
            {
                _spawner.SpawnInScreen(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Count)]);
                yield return new WaitForSeconds(_spawnSpeedInSeconds);
                if (_spawnSpeedInSeconds > MinSpeed)
                    _spawnSpeedInSeconds = SpeedUpDelayTime(_spawnSpeedInSeconds, SpeedUpValue);
            }
           
        }

        private float SpeedUpDelayTime(float time, float value)
        {
            return time - value;
        }

        #endregion


        #region PUN CALLBACKS

        public override void OnDisconnected(DisconnectCause cause)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Finish Menu");
        }

        public override void OnLeftRoom()
        {
            PhotonNetwork.Disconnect();
        }

        public override void OnMasterClientSwitched(Player newMasterClient)
        {
            if (PhotonNetwork.LocalPlayer.ActorNumber == newMasterClient.ActorNumber)
            {
                StartCoroutine(SpawnCountObjectsByTime());
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
                    Debug.Log("setting text waiting for players! ");
                }
            }
        }

        private bool CheckAllPlayerLoadedLevel()
        {
            foreach (Player p in PhotonNetwork.PlayerList)
            {
                object playerLoadedLevel;

                if (p.CustomProperties.TryGetValue(SnakeGame.PLAYER_LOADED_LEVEL, out playerLoadedLevel))
                {
                    if ((bool)playerLoadedLevel)
                    {
                        continue;
                    }
                }

                return false;
            }

            return true;
        }

        #endregion
    }
}