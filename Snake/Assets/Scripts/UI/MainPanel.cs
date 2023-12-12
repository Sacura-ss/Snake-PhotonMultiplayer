using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using Snake;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UI
{
    public class MainPanel : MonoBehaviourPunCallbacks
    {
        [SerializeField] private Button _startButton;
        [SerializeField] private TMP_Text _errorText;
        
        [Header("Room creating")]
        [SerializeField] private TMP_InputField _roomName;
        [SerializeField] private Button _createRoomButton;

        [Header("Room connecting")] 
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _contentTransform;

        private void OnEnable()
        {
            _startButton.onClick.AddListener(LoadGame);
            _createRoomButton.onClick.AddListener(CreateRoom);
        }
        
        private void OnDisable()
        {
            _startButton.onClick.RemoveListener(LoadGame);
            _createRoomButton.onClick.RemoveListener(CreateRoom);
        }

        private void LoadGame()
        {
            SavedData.CreatePlayers(1);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }

        private void CreateRoom()
        {
            if(!PhotonNetwork.IsConnected) return;
            
            if (_roomName.text == String.Empty)
            {
                _errorText.text = "Room Name Is Empty";
            }
            else
            {
                _errorText.text = "";
                RoomOptions roomOptions = new RoomOptions
                {
                    MaxPlayers = 4,
                    IsVisible = true
                };

                PhotonNetwork.CreateRoom(_roomName.text, roomOptions, TypedLobby.Default);
                // if(PhotonNetwork.CreateRoom(_roomName.text, roomOptions, TypedLobby.Default))
                //     PhotonNetwork.LoadLevel("Game");
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("UPDATE LIST");
            foreach (var roomInfo in roomList)
            {
                RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _contentTransform);
                if (roomListItem != null)
                {
                    roomListItem.SetInfo(roomInfo);
                }
            }
        }
    }
}