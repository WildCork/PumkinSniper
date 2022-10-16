using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;



public class CharacterBase : AllObject
{
    public enum Direction { Left, Right }
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DetectGround m_detectGround;

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
    [SerializeField] private int _bulletCnt = 0;
    [SerializeField] private FirearmBase.FirearmKind _firemArm = FirearmBase.FirearmKind.Pistol;

    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _sneakSpeed = 0;
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


    private void Descend()
    {
        _detectGround.Descend();
    }

    #region Trigger Door

    [SerializeField] private List<Collider2D> _triggerWallSet = new();

    private float _dotValue;
    private void OnTriggerEnter2D(Collider2D collision)//Door와 겹치는 순간은 InDoor로 분류
    {
        if (collision.gameObject.layer == GameManager.s_instance._doorLayer && _triggerWallSet.Count == 0)
        {
            Vector2 doorDir = collision.transform.rotation * Vector2.left; //From Out To In
            _dotValue = Vector2.Dot(ContactNormalVec(collision, transform.position), doorDir);
            switch (_locationStatus)
            {
                case LocationStatus.In:
                    if (_dotValue < 0)
                    {
                        RefreshLocationStatus(LocationStatus.Door);
                    }
                    break;
                case LocationStatus.Out:
                    if (_dotValue > 0)
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
        else if (collision.gameObject.layer == GameManager.s_instance._wallLayer)
        {
            _triggerWallSet.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision) //TODO: check IndoorOutdoor
    {
        if (collision.gameObject.layer == GameManager.s_instance._doorLayer && _triggerWallSet.Count == 0)
        {
            Vector2 doorDir = collision.transform.rotation * Vector2.left; //From Out To In
            _dotValue = Vector2.Dot(ContactNormalVec(collision, transform.position), doorDir);
            switch (_locationStatus)
            {
                case LocationStatus.In:
                    break;
                case LocationStatus.Out:
                    break;
                case LocationStatus.Door:
                    if (_dotValue > 0)
                    {
                        RefreshLocationStatus(LocationStatus.Out);
                    }
                    else if (_dotValue < 0)
                    {
                        RefreshLocationStatus(LocationStatus.In);
                    }
                    break;
                default:
                    Debug.LogError($"It is no enum state for {_locationStatus}");
                    break;
            }
        }
        else if (collision.gameObject.layer == GameManager.s_instance._wallLayer)
        {
            _triggerWallSet.Remove(collision);
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

    private void RefreshLocationStatus(LocationStatus locationStatus)
    {
        _locationStatus = locationStatus;
        GameManager.s_instance.RenewMap();
    }
    private Vector2 ContactNormalVec(Collider2D collision, Vector2 pos)
    {
        return (collision.ClosestPoint(pos) - pos).normalized;
    }

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
}
