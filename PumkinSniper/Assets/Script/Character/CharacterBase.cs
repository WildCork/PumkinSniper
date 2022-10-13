using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class CharacterBase : AllObject
{
    public enum Direction { Left, Right }
    [SerializeField] private CameraController _cameraController;

    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _sneakSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0, 15)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Stats")]
    [SerializeField] private int _hp = 100;
    [SerializeField] private int _maxHp = 100;
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
            Debug.Log(_hp);
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


    private float _dotValue;
    [Tooltip("문 입장 허용 노말 벡터 크기")]
    private const float c_standardToEnterDoor = 0.7f;
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


    #region Non Physic Part


    private void Move(ref float horizontalInput, ref bool walkInput)
    {
        if (walkInput)
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
    #endregion

    #region Physics Part
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

    #region Special Event
    private void Die()
    {

    }
    #endregion

    #region Penetrate, Materialize

    private void Descend()
    {
        Penetrate();
    }
    public void Penetrate()
    {
        _collider2D.isTrigger = true;
    }

    public void Materialize()
    {
        _collider2D.isTrigger = false;
    }
    #endregion

    #region Collision, Trigger
    private void OnCollisionEnter2D(Collision2D collision)
    {
        switch (_locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    RefreshOnGround(true);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    RefreshOnGround(true);
                }
                break;
            case LocationStatus.Door:
                RefreshOnGround(true);
                break;
            default:
                break;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        switch (_locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    RefreshOnGround(false);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    RefreshOnGround(false);
                }
                break;
            case LocationStatus.Door:
                break;
            default:
                break;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)//Door와 겹치는 순간은 InDoor로 분류
    {
        if (collision.gameObject.layer == _wallLayer)
        {
            Materialize();
        }
        else if (collision.gameObject.layer == _doorLayer)
        {
            if (!ReferenceEquals(_doorObject, collision.gameObject))
            {
                _doorObject = collision.gameObject;
                _doorScript = _doorObject.GetComponent<Door>();
            }
            _dotValue = Vector2.Dot(ContactNormalVec(collision, transform.position), _doorScript._doorDirection);
            switch (_locationStatus)
            {
                case LocationStatus.In:
                    if (_dotValue < -c_standardToEnterDoor)
                    {
                        RefreshLocationStatus(LocationStatus.Door);
                    }
                    break;
                case LocationStatus.Out:
                    if (_dotValue > c_standardToEnterDoor)
                    {
                        RefreshLocationStatus(LocationStatus.Door);
                    }
                    break;
                case LocationStatus.Door:
                    break;
                default:
                    Debug.LogError($"It is no enum state for {_locationStatus}");
                    break;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //TODO: check IndoorOutdoor
    {
        if (collision.gameObject.layer == _doorLayer)
        {
            if (!ReferenceEquals(_doorObject, collision.gameObject))
            {
                _doorObject = collision.gameObject;
                _doorScript = _doorObject.GetComponent<Door>();
            }
            _dotValue = Vector2.Dot(ContactNormalVec(collision, transform.position), _doorScript._doorDirection);
            switch (_locationStatus)
            {
                case LocationStatus.Out:
                    break;
                case LocationStatus.In:
                    break;
                case LocationStatus.Door:
                    if (_dotValue > c_standardToEnterDoor)
                    {
                        RefreshLocationStatus(LocationStatus.Out);
                    }
                    else if (_dotValue < -c_standardToEnterDoor)
                    {
                        RefreshLocationStatus(LocationStatus.In);
                    }
                    break;
                default:
                    Debug.LogError($"It is no enum state for {_locationStatus}");
                    break;
            }
        }
        else
        {
            switch (_locationStatus)
            {
                case LocationStatus.Out:
                    if (collision.gameObject.layer == _outLayer)
                    {
                        Materialize();
                    }
                    break;
                case LocationStatus.In:
                    if (collision.gameObject.layer == _inLayer)
                    {
                        Materialize();
                    }
                    break;
                default:
                    break;
            }
        }
    }

    public void RefreshOnGround(bool value)
    {
        _isOnGround = value;
        if(_isOnGround)
        {
            InputController.s_instance._jumpUp = false;
            _isJump = false;
            _isStopJump = false;
            _onJumpTime = 0f;
        }
    }

    #endregion

}
