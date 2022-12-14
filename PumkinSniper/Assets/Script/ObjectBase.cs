using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;
using Photon.Realtime;
using System;
using static GameManager;

public class ObjectBase : MonoBehaviourPunCallbacks
{
    public enum LocationStatus { In, Out, Door };

    [Header("ObjectBase")]
    public LocationStatus _locationStatus = LocationStatus.Out;

    private SpriteRenderer m_spriteRenderer = null;
    private Rigidbody2D m_rigidbody2D = null;
    private Collider2D m_collider2D = null;
    private PhotonView m_photonView = null;
    private ParticleSystem m_particleSystem = null;

    public List<Collider2D> _triggerWallSet = new();
    public List<Collider2D> _triggerMapSet = new();

    protected Rigidbody2D _rigidbody2D
    {
        get
        {
            if (!m_rigidbody2D) { Debug.LogError($"{gameObject} Object doesn't have rigidbody component"); }
            return m_rigidbody2D;
        }
        set { m_rigidbody2D = value; }
    }

    protected Collider2D _collider2D
    {
        get
        {
            if (!m_collider2D) { Debug.LogError($"{gameObject} Object doesn't have collider component"); }
            return m_collider2D;
        }
        set { m_collider2D = value; }
    }

    protected SpriteRenderer _spriteRenderer
    {
        get
        {
            if (!m_spriteRenderer) { Debug.LogError($"{gameObject} Object doesn't have SpriteRenderer component"); }
            return m_spriteRenderer;
        }
        set { m_spriteRenderer = value; }
    }

    protected PhotonView _photonView
    {
        get
        {
            if (!m_photonView) { Debug.LogError($"{gameObject} Object doesn't have photonView component"); }
            return m_photonView;
        }
        set { m_photonView = value; }
    }

    protected ParticleSystem _particleSystem
    {
        get
        {
            if (!m_particleSystem) { Debug.LogError($"{gameObject} Object doesn't have photonView component"); }
            return m_particleSystem;
        }
        set { m_particleSystem = value; }
    }

    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        m_photonView = GetComponent<PhotonView>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_particleSystem = GetComponent<ParticleSystem>();
    }


    #region In Out Logic

    private Vector2 doorDir; //From Out To In
    private float _dotValue;
    private ObjectBase _object;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == gameManager._doorLayer && _triggerWallSet.Count == 0)
        {
            doorDir = collision.transform.rotation * Vector2.left;
            _dotValue = Vector2.Dot(ContactNormalVec(collision.transform.position, transform.position), doorDir);
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

        if (_object = collision.GetComponent<ObjectBase>())
        {
            if (_object._locationStatus == LocationStatus.Door)
            {
                Hit(collision);
            }
            else if (_object._locationStatus == _locationStatus)
            {
                Hit(collision);
            }
        }

        if (collision.gameObject.layer == gameManager._wallLayer
            || collision.gameObject.layer == gameManager._inLayer
            || collision.gameObject.layer == gameManager._outLayer)
        {
            _triggerMapSet.Add(collision);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == gameManager._doorLayer && _triggerWallSet.Count == 0)
        {
            doorDir = collision.transform.rotation * Vector2.left; //From Out To In
            _dotValue = Vector2.Dot(ContactNormalVec(collision.transform.position, transform.position), doorDir);
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
        
        if (collision.gameObject.layer == gameManager._wallLayer
            || collision.gameObject.layer == gameManager._inLayer
            || collision.gameObject.layer == gameManager._outLayer)
        {
            _triggerMapSet.Remove(collision);
        }
    }

    protected virtual void RefreshLocationStatus(LocationStatus locationStatus)
    {
        _locationStatus = locationStatus;
    }
    private Vector2 ContactNormalVec(Vector2 collision, Vector2 pos)
    {
        return (collision - pos).normalized;
    }
    protected virtual void Hit(Collider2D collsion) { }

    #endregion
}
