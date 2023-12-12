using System;
using System.Collections.Generic;
using GameLogic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DisplayScripts.Services
{
    public class Spawner : MonoBehaviour
    {
        [SerializeField] private Transform _parentForSpawnedObject;
        
        private Vector3 _topRight;
        private Vector3 _bottomLeft;

        public SpawnObject SpawnInScreen(SpawnObject gameObjectForSpawn)
        {
            Vector2 randomPositionOnScreen = Camera.main.ViewportToWorldPoint(new Vector2(Random.value, Random.value));
            
            //limit to the screen
            _topRight = Camera.main.ViewportToWorldPoint(Vector3.one);//new Vector2(Screen.width, Screen.height);
            _bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);//new Vector2(0, 0); 
            if (gameObjectForSpawn.TryGetComponent(out SpriteRenderer spriteRenderer))
            {
                if (spriteRenderer.sprite != null)
                {
                    var extents = spriteRenderer.sprite.bounds.extents;
                    randomPositionOnScreen.x = Mathf.Clamp(randomPositionOnScreen.x, _bottomLeft.x + extents.x, _topRight.x - extents.x);
                    randomPositionOnScreen.y = Mathf.Clamp(randomPositionOnScreen.y, _bottomLeft.y + extents.y, _topRight.y - extents.y);
                }
            }
    
            SpawnObject spawnedObject =
                Instantiate(gameObjectForSpawn, randomPositionOnScreen, Quaternion.identity, _parentForSpawnedObject);
    
            return spawnedObject;
        }
    
    }
}