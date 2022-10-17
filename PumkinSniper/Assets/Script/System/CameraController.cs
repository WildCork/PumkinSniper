using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private CharacterBase _character;
    private Camera _camera;

    [SerializeField] private float _cameraSize;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _maxSpeed;
    private Vector2 _currentPos;
    private Vector2 _targetPos;
    private Vector2 _velocity;
    private Vector3 _pos;

    void Start()
    {
        _camera = GetComponent<Camera>();

        _camera.orthographicSize = _cameraSize;
        _character = GameManager.s_instance._character;
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
