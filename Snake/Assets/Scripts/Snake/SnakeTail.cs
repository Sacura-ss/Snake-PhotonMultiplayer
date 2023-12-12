using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SnakeTail : MonoBehaviour
{
    [SerializeField] private Transform _tailTransform;
    [SerializeField] private float _circleDiameter;

    private List<Transform> _tailTransforms = new();
    private List<Vector2> _tailPositions = new();

    private void Start()
    {
        _tailPositions.Add(_tailTransform.position);
    }

    private void Update()
    {
        float distance = ((Vector2)_tailTransform.position - _tailPositions[0]).magnitude;

        if (distance > _circleDiameter)
        {
            Vector2 direction = ((Vector2)_tailTransform.position - _tailPositions[0]).normalized;
            
            _tailPositions.Insert(0, _tailPositions[0] + direction*_circleDiameter);
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
        Transform tail = Instantiate(_tailTransform, _tailPositions[_tailPositions.Count-1], Quaternion.identity, transform);
        _tailTransforms.Add(tail);
        _tailPositions.Add(tail.position);
    }
    
    
}
