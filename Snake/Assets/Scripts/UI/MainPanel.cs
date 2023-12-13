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
        [SerializeField] private TMP_Text _errorText;
        
        [Header("Room creating")]
        [SerializeField] private TMP_InputField _roomName;
        [SerializeField] private Button _createRoomButton;

        [Header("Room connecting")] 
        [SerializeField] private RoomListItem _roomListItemPrefab;
        [SerializeField] private Transform _contentTransform;

        public override void OnEnable()
        {
            _createRoomButton.onClick.AddListener(CreateRoom);
        }

        public override void OnDisable()
        {
            _createRoomButton.onClick.RemoveListener(CreateRoom);
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
                    IsVisible = true,
                    IsOpen = true
                };

                PhotonNetwork.CreateRoom(_roomName.text, roomOptions, TypedLobby.Default);
            }
        }

        public override void OnRoomListUpdate(List<RoomInfo> roomList)
        {
            Debug.Log("UPDATE LIST");
        }
        // public override void OnRoomListUpdate(List<RoomInfo> roomList)
        // {
        //     Debug.Log("UPDATE LIST");
        //     foreach (var roomInfo in roomList)
        //     {
        //         RoomListItem roomListItem = Instantiate(_roomListItemPrefab, _contentTransform);
        //         if (roomListItem != null)
        //         {
        //             roomListItem.SetInfo(roomInfo);
        //         }
        //     }
        // }
    }
}