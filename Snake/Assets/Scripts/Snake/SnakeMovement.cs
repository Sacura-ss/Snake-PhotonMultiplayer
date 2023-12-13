using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 3f;
    [SerializeField] private float _rotationSpeed = 200f;
    [SerializeField] private float _saveBounds = 2f;

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

            CheckIfOverScreen();
        }
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
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
