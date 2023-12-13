using System;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _playerCountInRoomText;
        private Button _button;

        private void OnEnable()
        {
            _button.onClick.AddListener(JoinToListRoom);
        }

        private void OnDisable()
        {
            _button.onClick.RemoveListener(JoinToListRoom);
        }

        public void SetInfo(RoomInfo roomInfo)
        {
            _roomNameText.text = roomInfo.Name;
            _playerCountInRoomText.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
        }

        private void JoinToListRoom()
        {
            PhotonNetwork.JoinRoom(_roomNameText.text);
        }
    }
}