using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Photon.Pun;
using static Bullet;
using static Grenade;
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

    public Vector3 ShootPos
    {
        get
        {
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
    public Vector3 ThrowPos
    {
        get
        {
            switch (direction)
            {
                case Direction.Left:
                    return transform.position + Vector3.left + Vector3.up;
                case Direction.Right:
                    return transform.position + Vector3.right + Vector3.up;
                default:
                    return transform.position;
            }
        }
    }

    public BulletType currentBulletType
    {
        get { return _bulletType; }
        set
        {
            if (_bulletType != value)
            {
                _bulletType = value;
                Debug.Log($"{_bulletType} Equipped!!");
            }
        }
    }

    public int IsShootUpDown
    {
        get { return _isShoorUpDown; }
        set
        {
            _isShoorUpDown = (value > 0 ? 1 : -1);
        }
    }


    [Header("Character Stats")]
    [SerializeField] private int _hp = 100;
    [SerializeField] private int _maxHp = 100;
    [SerializeField] private float _shootCancelDelay = 0.1f;
    [SerializeField] private int _maxBulletCnt = 300;
    [SerializeField] private int _bulletCnt = -1;
    [SerializeField] private BulletType _bulletType = BulletType.Pistol;
    [SerializeField] private int _maxGrenadeCnt = 100;
    [SerializeField] private int _grenadeCnt = 0;
    [SerializeField] private GrenadeType _grenadeType = GrenadeType.None;

    [Header("Character Ability")]
    [Range(0, 10)]
    [SerializeField] private float _walkSpeed = 0;
    [Range(0, 10)]
    [SerializeField] private float _runSpeed = 0;
    [Range(0, 15)]
    [SerializeField] private float _jumpPower = 0;
    [Range(0, 1)]
    [SerializeField] private float _canShortJumpTime;
    [Range(0, 10)]
    [SerializeField] private float _forceToBlockJump;

    [Header("Character Condition")]
    [SerializeField] private bool _isJump = false;
    [SerializeField] private bool _isShoot = false;
    [SerializeField] private bool _isStopJump = false;
    [SerializeField] private bool _isOnGround = false;
    [SerializeField] private float _onJumpTime = 0f;
    [SerializeField] private float _currentShootDelay = 0f;
    [SerializeField] private float _currentThrowDelay = 0f;
    public float CurrentShootDelay
    {
        get { return _currentShootDelay; }
        set
        {
            if (value > 0)
            {
                _currentShootDelay = value;
            }
            else
            {
                _currentShootDelay = 0;
            }
        }
    }

    public float CurrentThrowDelay
    {
        get { return _currentThrowDelay; }
        set
        {
            if (value > 0)
            {
                _currentThrowDelay = value;
            }
            else
            {
                _currentThrowDelay = 0;
            }
        }
    }

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
            else if(value < _hp)
            {
                Damaged(_hp - value);
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
            else if(value <= 0)
            {
                value = -1;
            }
            _bulletCnt = value;
        }
    }
    public int GrenadeCnt
    {
        get { return _grenadeCnt; }
        set
        {
            if (value > _maxGrenadeCnt)
            {
                value = _maxGrenadeCnt;
            }
            else if(value < 0)
            {
                value = 0;
            }
            _grenadeCnt = value;
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
        CurrentShootDelay -= Time.deltaTime;
        CurrentThrowDelay -= Time.deltaTime;
        if (_isJump && !_isStopJump)
        {
            _onJumpTime += Time.deltaTime;
        }
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            return;
        }

        if (_isOnGround)
        {
            Turn(ref inputController._horizontal);
            if (inputController._descend)
            {
                Descend();
            }
        }
        TryShoot();
        TryThrowGrenade();
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
            if((horizontalInput > 0) != (preLocalScale.x > 0))
            {
                preLocalScale.x = (horizontalInput > 0 ? 1 : -1);
            }
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

    private void TryShoot()
    {
        if (gameManager._bulletStorage[_bulletType].Count == 0)
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
            if (CurrentShootDelay > _shootCancelDelay)
            {
                CurrentShootDelay = _shootCancelDelay;
            }
        }

        if (_isShoot)
        {
            Shoot();
        }
    }

    private int _isShoorUpDown = 1;
    private void Shoot()
    {
        if (CurrentShootDelay > 0)
        {
            return;
        }
        BulletCnt--;
        gameManager._bulletStorage[_bulletType][0].Shoot(this);
        if (BulletCnt < 0)
        {
            BackToDefaultWeapon();
        }
    }

    private void BackToDefaultWeapon()
    {
        if (_bulletType != BulletType.Pistol)
        {
            _bulletType = BulletType.Pistol;
        }
    }

    #endregion

    #region Throw Grenade

    private void TryThrowGrenade()
    {
        if (inputController._grenadeDown)
        {
            if (_grenadeType == GrenadeType.None)
            {
                return;
            }
            ThrowGrenade();
        }
    }

    private void ThrowGrenade()
    {
        if (CurrentThrowDelay > 0)
        {
            return;
        }
        GrenadeCnt--;
        gameManager._grenadeStorage[_grenadeType][0].Throw(this);
    }

    #endregion


    #region Special Event
    private void Die()
    {

    }

    private void Damaged(int damageValue)
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
