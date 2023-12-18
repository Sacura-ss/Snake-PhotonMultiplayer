using System.Collections;
using DisplayScripts.Services;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace GameLogic
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpawnObject : MonoBehaviourPunCallbacks
    {
        [Header("Spawn Settings")] [SerializeField]
        private float _targetSizeOfSpawnedObject = 1.2f;

        [SerializeField] private float _disappearanceSpeedInSeconds = 6f;
        [SerializeField] private float SpeedUpValue = 0.2f;
        [SerializeField] private float MinSpeed = 2f;

        private bool isDestroyed;
        private PhotonView photonView;
        private Collider2D _collider;

        private IEnumerator _disappearCoroutine;

        public void Awake()
        {
            photonView = GetComponent<PhotonView>();
            _collider = GetComponent<Collider2D>();
        }

        public void Start()
        {
            if (!photonView.IsMine) return;
            
            if (_disappearCoroutine != null)
                StopCoroutine(_disappearCoroutine);
            _disappearCoroutine = EnableDisappearanceObjects();
            StartCoroutine(_disappearCoroutine);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {
            if (isDestroyed)
            {
                return;
            }
        
            if (other.gameObject.TryGetComponent(out SnakePlayer snake))
            {
                if (photonView.IsMine)
                {
                    if (TryGetComponent(out Food.Food food))
                    {
                        DestroySpawnObjectGlobally();
                        snake.gameObject.GetComponent<PhotonView>().RPC("AddTail", RpcTarget.All, snake.name);
                        //snake.AddTail();
                    }
                    else if (TryGetComponent(out Food.BadFood badFood))
                    {
                      
                        badFood.PlayVFX();
                        DestroySpawnObjectGlobally();
                        other.gameObject.GetComponent<PhotonView>().RPC("DestroySnake", RpcTarget.All);
                    }
                }
                else
                {
                    DestroySpawnObjectLocally();
                }
            }
        }

        private void Destroy()
        {
            if (isDestroyed) return;

            if (photonView.IsMine)
            {
                DestroySpawnObjectGlobally();
            }
            else
            {
                DestroySpawnObjectLocally();
            }
        }

        private void DestroySpawnObjectGlobally()
        {
            isDestroyed = true;

            PhotonNetwork.Destroy(gameObject);
        }

        private void DestroySpawnObjectLocally()
        {
            isDestroyed = true;

            _collider.enabled = false;
            GetComponent<Renderer>().enabled = false;
            foreach (var renderer in GetComponentsInChildren<Renderer>())
            {
                renderer.enabled = false;
            }
        }

        private IEnumerator EnableDisappearanceObjects()
        {
            Debug.Log("DIS " + gameObject);
            TryDisappearance(gameObject);

            yield return new WaitForSeconds(_disappearanceSpeedInSeconds);

            Destroy();

            if (_disappearanceSpeedInSeconds > MinSpeed)
                _disappearanceSpeedInSeconds = SpeedUpDelayTime(_disappearanceSpeedInSeconds, SpeedUpValue);
        }

        private void TryDisappearance(GameObject objectForDisappearance)
        {
            // if (objectForDisappearance.TryGetComponent(out DisappearanceObject disappearanceObject))
            // {
            //     disappearanceObject.Resize(_disappearanceSpeedInSeconds, _targetSizeOfSpawnedObject);
            //     if (disappearanceObject.TryGetComponent(out Image image))
            //     {
            //         disappearanceObject.FadeOut(image, _disappearanceSpeedInSeconds);
            //     }
            // }
        }

        private float SpeedUpDelayTime(float time, float value)
        {
            return time - value;
        }
    }
}