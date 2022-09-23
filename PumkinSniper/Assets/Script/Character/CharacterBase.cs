using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : AllObject
{
    [Header("Character Ability")]
    [Range(0,10)]
    [SerializeField] private float _moveSpeed = 0;
    [Range(0,20)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Stats")]
    [SerializeField] private float _flyingTime = 0;
    [SerializeField] private float _canShortJumpTime = 0.2f;
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isOnGround = false;
    // Start is called before the first frame update
    
    private void Update()
    {
        Walk(InputController.s_instance._horizontalInput);
        Jump(InputController.s_instance._jumpUpInput, InputController.s_instance._jumpDownInput);
        GoDown(InputController.s_instance._goDownInput);
    }

    private void Walk(float _horizontalInput)
    {
        if (!_isOnGround || _horizontalInput == 0)
        {
            return;
        }
        Vector2 preVelocity = _rigidbody2D.velocity;
        preVelocity.x = _horizontalInput * _moveSpeed;
        _rigidbody2D.velocity = preVelocity;
    }

    private void Jump(bool _jumpUpInput, bool _jumpDownInput)
    {
        if (_isOnGround && _jumpDownInput)
        {
            _isJump = true;
            _rigidbody2D.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
        }

        if (!_isJump || _flyingTime > _canShortJumpTime)
        {
            return;
        }

        _flyingTime += Time.deltaTime;
        if (_jumpUpInput)
        {
            _rigidbody2D.AddForce(Vector2.down * _jumpPower * 0.5f, ForceMode2D.Impulse);
        }
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
        _flyingTime = 0;
        _isJump = false;
        _collider2D.enabled = true;
    }
}
