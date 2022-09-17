using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBase : MonoBehaviour
{
    private Rigidbody2D _rigidBody;
    private CircleCollider2D _circleCollider2D;

    [Header("Character Ability")]
    [Range(0,10)]
    [SerializeField] private float _moveSpeed = 0;
    [Range(0,20)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Stats")]
    [SerializeField] private float _flyingTime = 0;
    [SerializeField] private float _canShortJumpTime = 0;
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isOnGround = false;
    // Start is called before the first frame update
    private void Awake()
    {
        _canShortJumpTime = 0.2f;
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            Vector2 preVelocity = _rigidBody.velocity;
            preVelocity.x = Input.GetAxis("Horizontal") * _moveSpeed;
            _rigidBody.velocity = preVelocity;
        }

        if (_isOnGround)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                _isJump = true;
                _rigidBody.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Penetrate();
            }


            if (_isJump)
            {
                _flyingTime += Time.deltaTime;
            }
        }

        if(_isJump && _flyingTime < _canShortJumpTime)
        {
            if(Input.GetKeyUp(KeyCode.Space))
            {
                _rigidBody.velocity *= 0.5f;
            }
        }
    }

    public void Penetrate()
    {
        _isOnGround = false;
        _circleCollider2D.enabled = false;
    }

    public void Materialize()
    {
        _isOnGround = true;
        _flyingTime = 0;
        _isJump = false;
        _circleCollider2D.enabled = true;
    }
}
