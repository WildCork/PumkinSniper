using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class CharacterBase : ObjectBase
{
    public enum Direction { Left, Right }
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DetectGround m_detectGround;
    [SerializeField] private Vector2 _shootPos;

    
    private DetectGround _detectGround
    {
        get 
        {
            if (!m_detectGround)
            {
                m_detectGround = transform.Find("DetectGround").GetComponent<DetectGround>();
            }
            return m_detectGround;
        }
    }
    [Header("Character Stats")]
    [SerializeField] private int _hp = 100;
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private FirearmBase.FirearmKind _firemArm = FirearmBase.FirearmKind.Pistol;

    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0, 15)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Condition")]
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isStopJump = false;
    [SerializeField] private bool _isOnGround = false;
    [Range(0, 1)]
    [SerializeField] private float _canShortJumpTime;
    [Range(0, 10)]
    [SerializeField] private float _forceToBlockJump;
    [SerializeField] private float _onJumpTime = 0f;

    public int HP
    {
        get { return _hp; }
        set 
        {
            if (value > _maxHp)
            {
                _hp = _maxHp;
            }
            else if(value <= 0)
            {
                _hp = 0;
                Die();
            }
            else
            {
                _hp = value;
            }
        }
    }


    public Direction direction
    {
        get { return transform.localScale.x > 0 ? Direction.Right : Direction.Left; }
    }

    private void FixedUpdate()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        Move(ref InputController.s_instance._horizontal, ref InputController.s_instance._walk);
        if (_isOnGround)
        {
            Turn(ref InputController.s_instance._horizontal);
            if (ReturnTrue_MakeFalse(ref InputController.s_instance._descend))
            {
                Descend();
            }
        }
        AllJump();
        if (InputController.s_instance._shoot)
        {
            Shoot();
        }
    }


    #region Move Part


    private void Move(ref float horizontalInput, ref bool walkInput)
    {
        if (walkInput && _isOnGround)
            Walk(ref horizontalInput);
        else
            Run(ref horizontalInput);
    }

    private void Walk(ref float horizontalInput)
    {
        transform.Translate(Vector2.right * horizontalInput * _walkSpeed * Time.deltaTime);
    }
    private void Run(ref float horizontalInput)
    {
        transform.Translate(Vector2.right * horizontalInput * _runSpeed * Time.deltaTime);
    }

    private void Turn(ref float horizontalInput)
    {
        Vector2 preLocalScale = transform.localScale;
        if (horizontalInput != 0)
        {
            preLocalScale.x = (horizontalInput > 0 ? 1 : -1);
        }
        transform.localScale = preLocalScale;
    }
    private void Descend()
    {
        _detectGround.Descend();
    }

    #endregion

    #region Jump Part

    private void AllJump()
    {
        if (_isOnGround)
        {
            if (!_isJump && ReturnTrue_MakeFalse(ref InputController.s_instance._jumpDown))
            {
                InputController.s_instance._jumpUp = false;
                Jump();
            }
        }
        else
        {
            if (_isJump && !_isStopJump)
            {
                _onJumpTime += Time.deltaTime;
                if (_onJumpTime < _canShortJumpTime && ReturnTrue_MakeFalse(ref InputController.s_instance._jumpUp))
                {
                    StopJump();
                }
            }
        }
    }
    private void Jump()
    {
        _isOnGround = false;
        _isJump = true;
        _isStopJump = false;
        _rigidbody2D.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void StopJump()
    {
        _isStopJump = true;
        _rigidbody2D.AddForce(Vector2.down * _forceToBlockJump, ForceMode2D.Impulse);
    }

    #endregion

    #region Shoot Part

    private void Shoot()
    {
        switch (_firemArm)
        {
            case FirearmBase.FirearmKind.Pistol:

                break;
            case FirearmBase.FirearmKind.Machinegun:
                break;
            case FirearmBase.FirearmKind.Shotgun:
                break;
            default:
                break;
        }
    }

    #endregion

    #region Special Event
    private void Die()
    {

    }
    #endregion




    #region Intellgience Function
    private bool ReturnTrue_MakeFalse(ref bool condition)
    {
        if (condition)
        {
            condition = false;
            return true;
        }
        else
        {
            return false;
        }
    }
    public void RefreshOnGround(bool value)
    {
        _isOnGround = value;
        if (_isOnGround)
        {
            InputController.s_instance._jumpUp = false;
            _isJump = false;
            _isStopJump = false;
            _onJumpTime = 0f;
        }
    }
    #endregion
}
