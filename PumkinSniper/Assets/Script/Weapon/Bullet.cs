using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using static CharacterBase;
using static GameManager;

public class Bullet : ObjectBase
{
    public enum BulletKind { Pistol, Machinegun, Shotgun }
    public BulletKind _bulletKind = BulletKind.Pistol;
    [Header("Time")]
    public float _shootDelayTime;
    public float _maxLifeTime;
    [SerializeField] private const float c_lifeCycleTime = 0.1f;
    [SerializeField] private float _lifeTimeAfterRebound;

    [Header("Rebound")]
    [Range(0,1)]
    [SerializeField] private float _reboundDamp;
    [Range(0,90)]
    [SerializeField] private float _reboundAngle;
    [Header("Stats")]
    public float _shootPosOffset;
    [SerializeField] private int _damage;
    [SerializeField] private float _shotSpeed;

    private bool _isRebound = false;
    private WaitForSeconds _lifeCycleSeconds = new WaitForSeconds(c_lifeCycleTime);
    private float _inStatusValueZ
    {
        get { return Map._walls.transform.position.z; }
    }
    private float _outStatusValueZ
    {
        get { return Map._outMap.transform.position.z; }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _collider2D.isTrigger = true;
        _isRebound = false;
        _rigidbody2D.drag = 0;
        _rigidbody2D.gravityScale = 0f;
    }

    public override void OnDisable()
    {
        transform.position = gameManager._storageTransform[_bulletKind].position;
        _triggerWallSet.Clear();
        gameManager._bulletStorage[_bulletKind].Add(this);
        base.OnDisable();
    }
    public void Shoot(Vector3 shootPos, Direction direction)
    {
        gameManager._bulletStorage[_bulletKind].RemoveAt(0);
        gameObject.SetActive(true);
        transform.position = shootPos;
        switch (direction)
        {
            case Direction.Left:
                _rigidbody2D.velocity = Vector2.left * _shotSpeed;
                break;
            case Direction.Right:
                _rigidbody2D.velocity = Vector2.right * _shotSpeed;
                break;
            default:
                break;
        }
        StartCoroutine(Fire());
    }

    private float _lifeTime = 0;
    IEnumerator Fire()
    {
        for (_lifeTime = 0; _lifeTime < _maxLifeTime; _lifeTime += c_lifeCycleTime)
        {
            yield return _lifeCycleSeconds;
        }
        gameObject.SetActive(false);
    }

    protected override void RefreshLocationStatus(LocationStatus locationStatus)
    {
        base.RefreshLocationStatus(locationStatus);
        Vector3 pos = transform.position;
        switch (locationStatus)
        {
            case LocationStatus.Out:
                pos.z = _outStatusValueZ;
                break;
            case LocationStatus.In:
            case LocationStatus.Door:
                pos.z = _inStatusValueZ;
                break;
            default:
                break;
        }
        transform.position = pos;
    }

    private CharacterBase _targetCharacter;
    protected override void Hit(Collider2D collision)
    {
        switch (_locationStatus)
        {
            case LocationStatus.In:
                if (collision.gameObject.layer == gameManager._wallLayer)
                {
                    ReboundFromWall(collision);
                }
                else if (collision.gameObject.layer == gameManager._inLayer)
                {
                    if (collision.gameObject.CompareTag(gameManager._bottomString))
                    {
                        Materialize();
                    }
                }
                break;
            case LocationStatus.Out:
                if (collision.gameObject.layer == gameManager._outLayer)
                {
                    if (collision.gameObject.CompareTag(gameManager._bottomString))
                    {
                        Materialize();
                    }
                }
                break;
            default:
                if (!_isRebound && collision.gameObject.layer == gameManager._playerLayer)
                {
                    _targetCharacter = collision.gameObject.GetComponent<CharacterBase>();
                    if (_targetCharacter._locationStatus == LocationStatus.Door)
                    {
                        HitCharacter(_targetCharacter);
                    }
                    else if(_targetCharacter._locationStatus == _locationStatus)
                    {
                        HitCharacter(_targetCharacter);
                    }
                }
                break;
        }
    }

    private void Materialize()
    {
        _collider2D.isTrigger = false;
    }

    [SerializeField] private ContactPoint2D[] contacts = new ContactPoint2D[10];
    private Vector2 reboundVec = Vector2.zero;
    private void ReboundFromWall(Collider2D collision)
    {
        _isRebound = true;
        collision.GetContacts(contacts);
        if (_lifeTime < _maxLifeTime - _lifeTimeAfterRebound)
        {
            _lifeTime = _maxLifeTime - _lifeTimeAfterRebound;
        }
        reboundVec = Quaternion.Euler(0, 0, Random.Range(-_reboundAngle, _reboundAngle)) * -_rigidbody2D.velocity;
        _rigidbody2D.velocity = reboundVec * _reboundDamp;
        _rigidbody2D.gravityScale = 1f;
        _rigidbody2D.drag = 1f;
    }

    private void HitCharacter(CharacterBase character)
    {
        character.HP -= _damage;
    }

}
