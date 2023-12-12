using UnityEngine;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 200f;

    private float velX = 0f;
    private bool _isStarted;
    private void Update()
    {
        if (Input.anyKey) _isStarted = true;
        if(_isStarted) velX = Input.GetAxisRaw("Horizontal");
    }

    private void FixedUpdate()
    {
        if (_isStarted)
        {
            transform.Translate(Vector3.up * _moveSpeed * Time.fixedDeltaTime, Space.Self);
        
            transform.Rotate(Vector3.forward * -velX * _rotationSpeed * Time.fixedDeltaTime);
        }
    }
}
