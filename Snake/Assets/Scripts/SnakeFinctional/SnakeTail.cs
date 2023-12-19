using System;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using UnityEngine;

namespace DefaultNamespace.SnakeFinctional
{
    public class SnakeTail: MonoBehaviour
    {
        private PhotonView _photonView;
        private void Awake()
        {
            _photonView = GetComponent<PhotonView>();
        }

        private void OnEnable()
        {
            if (TryGetComponent(out Renderer r))
                r.material.color = SnakeGame.GetColor(_photonView.Owner.GetPlayerNumber());
        }

        
        public void Disable()
        {
            if (!_photonView.IsMine) return;
            _photonView.RPC("Disabling", RpcTarget.AllViaServer);
            
        }
        
        public void Enable()
        {
            if (!_photonView.IsMine) return;
            _photonView.RPC("Enabling", RpcTarget.AllViaServer);
        }

        [PunRPC]
        private void Enabling()
        {
            if(TryGetComponent(out Renderer renderer))
                renderer.enabled = true;
        }

        [PunRPC]
        private void Disabling()
        {
            if(TryGetComponent(out Renderer renderer))
                renderer.enabled = false;
        }
    }
}