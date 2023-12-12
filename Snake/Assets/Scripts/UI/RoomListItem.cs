using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace UI
{
    public class RoomListItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _roomNameText;
        [SerializeField] private TMP_Text _playerCountInRoomText;

        public void SetInfo(RoomInfo roomInfo)
        {
            _roomNameText.text = roomInfo.Name;
            _playerCountInRoomText.text = roomInfo.PlayerCount + "/" + roomInfo.MaxPlayers;
        }
    }
}