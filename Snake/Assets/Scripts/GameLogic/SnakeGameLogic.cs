using System;
using System.Collections;
using System.Collections.Generic;
using DisplayScripts.Services;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GameLogic
{
    public class SnakeGameLogic : MonoBehaviour
    {
        [Header("Objects for game")]
        [SerializeField] private List<SpawnObject> _spawnPrefabs;

        [Header("Speed Settings")]
        [SerializeField] private float _spawnSpeedInSeconds = 1f;
        [SerializeField] private float _disappearanceSpeedInSeconds = 4f;
        [SerializeField] private float SpeedUpValue = 0.3f;
        [SerializeField] private float MinSpeed = 1f;
        
        [Header("Count Settings")]
        [SerializeField] private int _spawnObjectCount = 10;
        
        [Header("Size Settings")]
        [SerializeField] private float _targetSizeOfSpawnedObject = 2.5f;
        
        private Spawner _spawner;
        private Queue<GameObject> _objectsInGame = new();
        private Queue<SpawnObject> _spawnedObjects = new();
        
        public void StopGame()
        {
            StopAllCoroutines();
            foreach (var obj in _objectsInGame)
            {
                Destroy(obj);
            }
        }

        private void OnEnable()
        {
            Spawn();
        }

        private void OnDisable()
        {
            _spawnedObjects.Clear();
            _objectsInGame.Clear();
        }

        private void Start()
        {
            StartCoroutine(EnableCountObjectsByTime());
        }

        private void Spawn()
        {
            _spawner = FindObjectOfType<Spawner>();

            for (int i = 0; i < _spawnObjectCount; i++)
            {
                var spawned = _spawner.SpawnInScreen(_spawnPrefabs[Random.Range(0, _spawnPrefabs.Count)]);
                _spawnedObjects.Enqueue(spawned);
                spawned.transform.SetSiblingIndex(1);
            }
        }

        private IEnumerator EnableCountObjectsByTime()
        {
            while (_spawnedObjects.Count > 0)
            {
                StartCoroutine(EnableDisappearanceObjects());
                yield return new WaitForSeconds(_spawnSpeedInSeconds);
                if (_spawnSpeedInSeconds > MinSpeed)
                    _spawnSpeedInSeconds = SpeedUpDelayTime(_spawnSpeedInSeconds, SpeedUpValue);
                if (_disappearanceSpeedInSeconds > MinSpeed)
                    _disappearanceSpeedInSeconds = SpeedUpDelayTime(_disappearanceSpeedInSeconds, SpeedUpValue);
            }
        }

        private float SpeedUpDelayTime(float time, float value)
        {
            return time - value;
        }

        private IEnumerator EnableDisappearanceObjects()
        {
            var spawnObject = _spawnedObjects.Dequeue();
            spawnObject.gameObject.SetActive(true);

            _objectsInGame.Enqueue(spawnObject.gameObject);

            TryDisappearance(spawnObject.gameObject);

            yield return new WaitForSeconds(_disappearanceSpeedInSeconds);

            _objectsInGame.Dequeue();

            if (spawnObject != null)
            {
                Destroy(spawnObject.gameObject);
            }
        }

        private void TryDisappearance(GameObject objectForDisappearance)
        {
            if (objectForDisappearance.TryGetComponent(out DisappearanceObject disappearanceObject))
            {
                disappearanceObject.Resize(_disappearanceSpeedInSeconds, _targetSizeOfSpawnedObject);
                if (disappearanceObject.TryGetComponent(out Image image))
                {
                    disappearanceObject.FadeOut(image, _disappearanceSpeedInSeconds);
                }
            }
        }
    }
}