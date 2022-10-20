using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using static Bullet;
using static GameManager;
using static InputController;

public class CharacterBase : ObjectBase , IPunObservable
{
    public enum Direction { Left, Right }
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DetectGround m_detectGround;
    [SerializeField] private Transform m_shootPos;

    void IPunObservable.OnPhotonSerializeView(Photon.Pun.PhotonStream stream, Photon.Pun.PhotonMessageInfo info)
    {
        if (photonView.IsMine)
        {

        }
        else
        {

        }
    }

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

    private Vector3 _shootPos
    {
        get {
            switch (direction)
            {
                case Direction.Left:
                    return transform.position + Vector3.left; 
                case Direction.Right:
                    return transform.position + Vector3.right;
                default:
                    return transform.position;
            }
        }
    }



    [Header("Character Stats")]
    [SerializeField] private int _hp = 100;
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _shootCancelDelay = 0.1f;
    [SerializeField] private int _maxBulletCnt = 200;
    [SerializeField] private int _bulletCnt = -1;
    public BulletType _firemArm = BulletType.Pistol;

    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0, 15)]
    [SerializeField] private float _jumpPower = 0;

    [Header("Character Condition")]
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isShoot = false;
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

    public int BulletCnt
    {
        get { return _bulletCnt; }
        set 
        {
            if (value > _maxBulletCnt)
            {
                value = _maxBulletCnt;
            }
            _bulletCnt = value; 
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

        Move(ref inputController._horizontal, ref inputController._walk);
    }

    private void Update()
    {
        if (_isOnGround)
        {
            Turn(ref inputController._horizontal);
            if (inputController._descend)
            {
                Descend();
            }
        }
        TryShoot();
        AllJump();
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
            if (!_isJump && inputController._jumpDown)
            {
                inputController._jumpUp = false;
                Jump();
            }
        }
        else
        {
            if (_isJump && !_isStopJump)
            {
                _onJumpTime += Time.deltaTime;
                if (_onJumpTime < _canShortJumpTime && inputController._jumpUp)
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

    private float _shootOffset;
    private float _shootDelay = 0f;
    private int _shootUpOrDown = 1;
    private void TryShoot()
    {
        if (gameManager._bulletStorage[_firemArm].Count == 0)
        {
            Debug.LogError("There is no bullets!!");
            return;
        }

        if (inputController._shootDown)
        {
            _isShoot = true;
        }
        else if (inputController._shootUp)
        {
            _isShoot = false;
            if (_shootDelay > _shootCancelDelay)
            {
                _shootDelay = _shootCancelDelay;
            }
        }

        if (_isShoot)
        {
            if (_shootDelay <= 0)
            {
                Shoot();
            }
        }
        _shootDelay -= Time.deltaTime;
    }

    private void Shoot()
    {
        if (_bulletCnt > 0)
        {
            _bulletCnt--;
        }
        _shootUpOrDown = -_shootUpOrDown;
        _shootOffset = gameManager._bulletStorage[_firemArm][0]._shootPosOffset;
        gameManager._bulletStorage[_firemArm][0]._locationStatus = _locationStatus;
        gameManager._bulletStorage[_firemArm][0].Shoot(_shootPos + _shootUpOrDown * Vector3.up * _shootOffset, direction);
        _shootDelay = gameManager._bulletStorage[_firemArm][0]._shootDelayTime;
        if (_bulletCnt == 0)
        {
            _bulletCnt = -1;
            if (_firemArm != BulletType.Pistol)
            {
                _firemArm = BulletType.Pistol;
            }
        }
    }

    #endregion

    #region Special Event
    private void Die()
    {

    }
    #endregion




    #region Intellgience Function

    protected override void RefreshLocationStatus(LocationStatus locationStatus)
    {
        base.RefreshLocationStatus(locationStatus);
        gameManager.RenewMap();
    }
    public void RefreshOnGround(bool value)
    {
        _isOnGround = value;
        if (_isOnGround)
        {
            inputController._jumpUp = false;
            _isJump = false;
            _isStopJump = false;
            _onJumpTime = 0f;
        }
    }
    #endregion
}
