using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class CharacterBase : AllObject
{
    public enum Direction { Left, Right}
    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0,20)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Stats")]
    [SerializeField] private float _canShortJumpTime = 0;
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isOnGround = false;
    private float _speed;
    public Direction _direction
    {
        get
        {
            return transform.localScale.x > 0 ? Direction.Right : Direction.Left;
        }
    }
    // Start is called before the first frame update

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }
        Turn(InputController.s_instance._horizontalInput);
        Move(InputController.s_instance._horizontalInput, InputController.s_instance._runInput);
        Jump(InputController.s_instance._jumpDownInput);
        GoDown(InputController.s_instance._goDownInput);
    }

    private void Turn(float _horizontalInput)
    {
        if(_horizontalInput == 0)
        {
            return;
        }
        Vector2 preLocalScale = transform.localScale;
        preLocalScale.x = (_horizontalInput > 0 ? 1 : -1);
        transform.localScale = preLocalScale;
    }

    private void Move(float _horizontalInput, bool _runInput)
    {
        if (_horizontalInput == 0)
        {
            return;
        }
        else if (!_isOnGround)
        {
            return;
        }

        if (_runInput)
        {
            _speed = _runSpeed;
        }
        else
        {
            _speed = _walkSpeed;
        }

        Vector2 preVelocity = _rigidbody2D.velocity;
        if (Mathf.Abs(preVelocity.x) < _speed)
        {
            preVelocity.x += _horizontalInput;
        }
        else if (Mathf.Abs(preVelocity.x) > _speed)
        {
            preVelocity.x = (preVelocity.x > 0 ? 1 : -1) * _speed;
        }
        _rigidbody2D.velocity = preVelocity;
    }

    private void Jump(bool _jumpDownInput)
    {
        if (!_isOnGround || !_jumpDownInput)
        {
            return;
        }
        else if (_isJump)
        {
            return;
        }
        _isJump = true;
        _rigidbody2D.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void GoDown(bool _goDownInput)
    {
        if (!_isOnGround || !_goDownInput)
        {
            return;
        }
        Penetrate();
    }

    public void Penetrate()
    {
        _isOnGround = false;
        _collider2D.enabled = false;
    }

    public void Materialize()
    {
        _isOnGround = true;
        _isJump = false;
        _collider2D.enabled = true;
    }
}
