using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using static CharacterBase;
using static GameManager;


public class Grenade : ObjectBase
{
    public enum GrenadeType {None, Bomb }
    public GrenadeType _grenadeType = GrenadeType.Bomb;
    [Header("Time")]
    public float _maxDelayTime;
    public float _maxLifeTime;
    [SerializeField] private const float c_lifeCycleTime = 0.1f;


    [Header("Rebound")]
    [Range(0, 1)]
    [SerializeField] private float _reboundDamp;

    [Header("Stats")]
    [SerializeField] private int _damage;
    [SerializeField] private float _throwPower;
    [SerializeField] private Vector2 _throwVec;

    private WaitForSeconds _lifeCycleSeconds = new WaitForSeconds(c_lifeCycleTime);

    private Vector2 ThrowNormalVec
    {
        get { return _throwVec.normalized; }
    }

    public override void OnEnable()
    {
        base.OnEnable();
        _rigidbody2D.gravityScale = 1f;
    }

    public override void OnDisable()
    {
        transform.position = gameManager._grenadeStorageTransform[_grenadeType];
        _triggerWallSet.Clear();
        gameManager._grenadeStorage[_grenadeType].Add(this);
        _rigidbody2D.gravityScale = 0f;
        base.OnDisable();
    }

    public void Throw(CharacterBase character)
    {
        gameManager._grenadeStorage[_grenadeType].RemoveAt(0);

        character.CurrentThrowDelay = _maxDelayTime;

        gameObject.SetActive(true);
        _locationStatus = character._locationStatus;
        transform.position = character.ThrowPos;

        switch (character.direction)
        {
            case Direction.Left:
                if (_throwVec.x > 0)
                    _throwVec.x *= -1;
                break;
            case Direction.Right:
                if (_throwVec.x < 0)
                    _throwVec.x *= -1;
                break;
            default:
                break;
        }
        _rigidbody2D.velocity = character.GetComponent<Rigidbody2D>().velocity;
        _rigidbody2D.AddForce(_throwPower * ThrowNormalVec, ForceMode2D.Impulse);
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

    protected override void Hit(Collider2D collision)
    {
        base.Hit(collision);
        if (collision.gameObject.layer == gameManager._playerLayer)
        {

        }
        else
        {
            Rebound(collision);
        }
    }

    [SerializeField] private ContactPoint2D[] contacts = new ContactPoint2D[20];
    private Vector2 reboundVec = Vector2.zero;
    private void Rebound(Collider2D collision)
    {
        collision.GetContacts(contacts);
        //reboundVec = Quaternion.Euler(0, 0, Random.Range(-_reboundAngle, _reboundAngle)) * -_rigidbody2D.velocity;
        _rigidbody2D.velocity = reboundVec * _reboundDamp;
    }
}
