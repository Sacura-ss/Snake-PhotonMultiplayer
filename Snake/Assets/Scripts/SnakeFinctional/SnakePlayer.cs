using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using Photon.Pun;
using UnityEngine;

using Photon.Pun.UtilityScripts;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class SnakePlayer : MonoBehaviour
{
    [SerializeField] private Transform _tailTransform;
    [SerializeField] private float _circleDiameter;

    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 200f;
    [SerializeField] private float _saveBounds = 2f;
    
    private List<Transform> _tailTransforms = new();
    private List<Vector2> _tailPositions = new();
    private PhotonView _photonView;
    
    private new Collider2D collider;
    
    private float velX = 0f;
    private bool _isStarted;

    public void Awake()
    {
        _photonView = GetComponent<PhotonView>();
        
        collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>())
        {
            r.material.color = SnakeGame.GetColor(_photonView.Owner.GetPlayerNumber());
        }


        if (!_photonView.IsMine) return;
        _tailPositions.Add(_tailTransform.position);
    }
    
    private void Update()
    {
        if (!_photonView.AmOwner)
        {
            return;
        }

        // we don't want the master client to apply input to remote ships while the remote player is inactive
        if (this._photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber)
        {
            return;
        }

        if (Input.anyKey) _isStarted = true;
        
        if(_isStarted) velX = Input.GetAxisRaw("Horizontal");

        MoveTail();
     
    }
    
    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        
            return;
        
        if (!_isStarted) return;
        
        transform.Translate(Vector3.up * _moveSpeed * Time.fixedDeltaTime, Space.Self);
        
        transform.Rotate(Vector3.forward * -velX * _rotationSpeed * Time.fixedDeltaTime);

        CheckIfOverScreen();
    }

    private IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(SnakeGame.PLAYER_RESPAWN_TIME);

        _photonView.RPC("RespawnSnake", RpcTarget.AllViaServer);
    }
    
    private void CheckIfOverScreen()
    {
        var topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        var snakePosition = transform.position;

        if (snakePosition.x > topRight.x + _saveBounds
            || snakePosition.x < bottomLeft.x - _saveBounds
            || snakePosition.y > topRight.y + _saveBounds
            || snakePosition.y < bottomLeft.x - _saveBounds)
        {
            gameObject.GetComponent<PhotonView>().RPC("DestroySnake", RpcTarget.All);
        }
    }

    private void MoveTail()
    {
        float distance = ((Vector2)_tailTransform.position - _tailPositions[0]).magnitude;

        if (distance > _circleDiameter)
        {
            Vector2 direction = ((Vector2)_tailTransform.position - _tailPositions[0]).normalized;

            _tailPositions.Insert(0, _tailPositions[0] + direction * _circleDiameter);
            _tailPositions.RemoveAt(_tailPositions.Count - 1);

            distance -= _circleDiameter;
        }

        for (int i = 0; i < _tailTransforms.Count; i++)
        {
            _tailTransforms[i].position =
                Vector2.Lerp(_tailPositions[i + 1], _tailPositions[i], distance / _circleDiameter);
        }
    }

    public void AddTail()
    {
        if (!_photonView.IsMine) return;
        Transform tail = Instantiate(_tailTransform, _tailPositions[_tailPositions.Count - 1], Quaternion.identity,
            transform);
        _tailTransforms.Add(tail);
        _tailPositions.Add(tail.position);
    }
    
    [PunRPC]
    public void RespawnSnake()
    {
        collider.enabled = true;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }
        //gameObject.SetActive(true);
    }
    
    [PunRPC]
    public void DestroySnake()
    {
        collider.enabled = false;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }
        //gameObject.SetActive(false);
        
        if (_photonView.IsMine)
        {
            object lives;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SnakeGame.PLAYER_LIVES, out lives))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable {{SnakeGame.PLAYER_LIVES, ((int) lives <= 1) ? 0 : ((int) lives - 1)}});

                if (((int) lives) > 1)
                {
                    StartCoroutine("WaitForRespawn");
                }
            }
        }
    }
}