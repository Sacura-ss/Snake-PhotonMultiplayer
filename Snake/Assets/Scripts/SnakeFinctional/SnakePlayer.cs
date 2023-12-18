using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using GameLogic;
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

    public List<Transform> TailTransforms
    {
        get => _tailTransforms;
        set => _tailTransforms = value;
    }

    public List<Vector2> TailPositions
    {
        get => _tailPositions;
        set => _tailPositions = value;
    }

    private PhotonView _photonView;

    public PhotonView PhotonView
    {
        get => _photonView;
        set => _photonView = value;
    }

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
        if (!_photonView.AmOwner) return;

        // we don't want the master client to apply input to remote ships while the remote player is inactive
        if (this._photonView.CreatorActorNr != PhotonNetwork.LocalPlayer.ActorNumber) return;

        if (Input.anyKey && collider.enabled) _isStarted = true;

        if (_isStarted) velX = Input.GetAxisRaw("Horizontal");

        MoveTail();
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine) return;

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
            if (_photonView.IsMine)
            {
                gameObject.GetComponent<PhotonView>().RPC("DestroySnake", RpcTarget.All);
            }
        }
    }

    private void MoveTail()
    {
        Debug.Log("MoveTail");
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

    [PunRPC]
    public void AddTail(string snakePlayerName)
    {
        if (!_photonView.IsMine) return;
        SnakePlayer snakePlayer = GameObject.Find(snakePlayerName).GetComponent<SnakePlayer>();
        Debug.Log("AddTail");
        Transform tail = Instantiate(_tailTransform, _tailPositions[_tailPositions.Count - 1], Quaternion.identity,
            transform);
        // Transform tail = PhotonNetwork.InstantiateRoomObject(_tailTransform.name,
        //         snakePlayer.TailPositions[snakePlayer.TailPositions.Count - 1],
        //     Quaternion.identity, 0).transform;
        snakePlayer.TailTransforms.Add(tail);
        snakePlayer.TailPositions.Add(tail.position);
        tail.SetParent(snakePlayer.transform);
        if (tail.TryGetComponent(out Renderer r))
            r.material.color = SnakeGame.GetColor(snakePlayer.PhotonView.Owner.GetPlayerNumber());

        snakePlayer.PhotonView.Owner.AddScore(1);
    }

    [PunRPC]
    public void RespawnSnake()
    {
        transform.position = new Vector2(0, 0);
        transform.rotation = Quaternion.identity;

        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = true;
        }

        collider.enabled = true;
    }

    [PunRPC]
    public void DestroySnake()
    {
        collider.enabled = false;
        foreach (var renderer in GetComponentsInChildren<Renderer>())
        {
            renderer.enabled = false;
        }

        _isStarted = false;

        if (_photonView.IsMine)
        {
            object lives;
            if (PhotonNetwork.LocalPlayer.CustomProperties.TryGetValue(SnakeGame.PLAYER_LIVES, out lives))
            {
                PhotonNetwork.LocalPlayer.SetCustomProperties(new Hashtable
                    { { SnakeGame.PLAYER_LIVES, ((int)lives <= 1) ? 0 : ((int)lives - 1) } });

                if (((int)lives) > 1)
                {
                    StartCoroutine("WaitForRespawn");
                }
            }
        }
    }
}