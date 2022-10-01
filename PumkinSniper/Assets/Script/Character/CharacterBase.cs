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
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0, 15)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Stats")]
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isStopJump = false;
    [SerializeField] private bool _isOnGround = false;

    [Header("Constant Values")]
    [Range(0, 1)]
    [SerializeField] private float _canShortJumpTime;
    [Range(0, 10)]
    [SerializeField] private float _forceToBlockJump;
    private float _onJumpTime = 0f;
    private Vector2 _characterDir;


    private bool OnGround
    {
        get
        {
            if (_isOnGround)
            {
                _isJump = false;
                _isStopJump = false;
                _onJumpTime = 0f;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private float _dotValue;
    private const float c_standardToEnterDoor = 0.7f; // 문 입장 허용 노말 벡터 크기
    public Direction direction
    {
        get { return transform.localScale.x > 0 ? Direction.Right : Direction.Left; }
    }
    protected override void Init()
    {
        //_cameraController = GetComponent<CameraController>(); // 미리 할당 -> 추후 탐색 로직으로 개선
        _cameraController._character = this;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        Control(ref InputController.s_instance._horizontalInput, ref InputController.s_instance._runInput,
            ref InputController.s_instance._jumpUpInput, ref InputController.s_instance._jumpDownInput,
            ref InputController.s_instance._descendInput);
    }

    private void Control(ref float horizontalInput, ref bool runInput, ref bool jumpUpInput, ref bool jumpDownInput, ref bool descendInput)
    {
        Turn(ref horizontalInput);


        if (OnGround)
        {
            if (!runInput)
                Walk(ref horizontalInput);
            else
                Run(ref horizontalInput);

            if (!_isJump && jumpDownInput)
                Jump();

            if (descendInput)
                Penetrate(); //Go Down
        }
        else
        {
            if (_isJump)
            {
                _onJumpTime += Time.deltaTime;
                if (!_isStopJump && jumpUpInput && _onJumpTime < _canShortJumpTime)
                {
                    _isStopJump = true;
                    StopJump();
                }
            }
        }
    }

    private void Walk(ref float horizontalInput)
    {
        RenewVelocity(RefreshType.RefreshX, horizontalInput * _walkSpeed);
    }
    private void Run(ref float horizontalInput)
    {
        RenewVelocity(RefreshType.RefreshX, horizontalInput * _runSpeed);
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

    private void Jump()
    {
        _isOnGround = false;
        _isJump = true;
        _isStopJump = false;
        _rigidbody2D.AddForce(Vector2.up * _jumpPower, ForceMode2D.Impulse);
    }

    private void StopJump()
    {
        _rigidbody2D.AddForce(Vector2.down * _forceToBlockJump, ForceMode2D.Impulse); //TODO: 상수 처리 필요
    }

    public void Penetrate()
    {
        _collider2D.isTrigger = true;
    }

    public void Materialize()
    {
        _collider2D.isTrigger = false;
    }
    public void RefreshOnGround(bool value)
    {
        _isOnGround = value;
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
            _characterDir = collision.transform.position - transform.position;
            _dotValue = Vector2.Dot(_characterDir.normalized, _doorScript._doorDirection);
            switch (_locationStatus)
            {
                case LocationStatus.In:
                    if (_dotValue < -c_standardToEnterDoor)
                    {
                        ChangeLocationStatus(LocationStatus.Door);
                    }
                    break;
                case LocationStatus.Out:
                    if (_dotValue > c_standardToEnterDoor)
                    {
                        ChangeLocationStatus(LocationStatus.Door);
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
            _characterDir = collision.transform.position - transform.position;
            _dotValue = Vector2.Dot(_characterDir.normalized, _doorScript._doorDirection);
            switch (_locationStatus)
            {
                case LocationStatus.Out:
                    break;
                case LocationStatus.In:
                    break;
                case LocationStatus.Door:
                    if (_dotValue > c_standardToEnterDoor)
                    {
                        ChangeLocationStatus(LocationStatus.Out);
                    }
                    else if (_dotValue < -c_standardToEnterDoor)
                    {
                        ChangeLocationStatus(LocationStatus.In);
                    }
                    break;
                default:
                    Debug.LogError($"It is no enum state for {_locationStatus}");
                    break;
            }
        }
    }

    private enum RefreshType { RefreshX, RefreshY}
    private void RenewVelocity(RefreshType refreshType ,float value) // velocity의 x 혹은 y 변화는 이 함수만 담당
    {
        Vector2 velocity = _rigidbody2D.velocity;
        switch (refreshType)
        {
            case RefreshType.RefreshX:
                velocity.x = value;
                break;
            case RefreshType.RefreshY:
                velocity.y = value;
                break;
            default:
                break;
        }
        _rigidbody2D.velocity = velocity;
    }

}
