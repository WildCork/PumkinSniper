using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [SerializeField] private float _throwPower;
    [SerializeField] private Vector2 _throwVec;

    private WaitForSeconds _lifeCycleSeconds = new WaitForSeconds(c_lifeCycleTime);

    private Vector2 ThrowNormalVec
    {
        get { return _throwVec.normalized; }
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
}
