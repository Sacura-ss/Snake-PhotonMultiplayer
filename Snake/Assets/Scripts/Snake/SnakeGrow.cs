using System;
using System.Collections;
using Mono.Collections.Generic;
using Snake;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeGrow : MonoBehaviour
{
    [SerializeField] private SnakeTail _snakeTail;

    private bool _isVisible;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent(out Food.Food food))
        {
            Destroy(other.gameObject, 0.02f);
            _snakeTail.AddTail();
            SavedData.GrowPlayerValue(0);
        }

        else if (other.TryGetComponent(out Food.BadFood badFood))
        {
            badFood.PlayVFX();
            Destroy(other.gameObject, 1);
            Destroy(gameObject);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    // private void OnBecameInvisible()
    // {
    //    
    // }
    
}
