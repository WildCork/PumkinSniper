using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] public CharacterBase _character;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private Vector2 _currentPos;
    [SerializeField] private Vector2 _targetPos;
    [SerializeField] private Vector2 _velocity;
    [SerializeField] private Vector3 _pos;

    private void OnValidate()
    {
        
    }
    void Start()
    {
        //TODO: Search Player character

    }


    void Update()
    {
        _currentPos = transform.position;
        _targetPos = _character.transform.position;
        _pos = Vector2.SmoothDamp(_currentPos, _targetPos, ref _velocity, _smoothTime, _maxSpeed);
        _pos.z = _character.transform.position.z - 1;

        transform.position = _pos;
    }
}
