using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;

public class CameraController : MonoBehaviour
{
    private CharacterBase _character;
    private Camera _camera;

    [SerializeField] private float _cameraSize;
    [SerializeField] private float _smoothTime;
    [SerializeField] private float _maxSpeed;
    private const float cameraPosValueZ = -50;
    private Vector3 _targetPos;
    private Vector3 _velocity;

    void Start()
    {
        _camera = GetComponent<Camera>();

        _camera.orthographicSize = _cameraSize;
        _character = gameManager._character;
    }


    void Update()
    {
        _targetPos = _character.transform.position;
        _targetPos.z = cameraPosValueZ;
        transform.position = Vector3.SmoothDamp(transform.position, _targetPos, ref _velocity, _smoothTime, _maxSpeed);
    }
}
