using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class AllObject : MonoBehaviourPunCallbacks
{
    public enum LocationStatus { In, Out, Door };

    public LocationStatus _locationStatus = LocationStatus.Out;

    private SpriteRenderer m_spriteRenderer = null;
    private Rigidbody2D m_rigidbody2D = null;
    private Collider2D m_collider2D = null;
    private PhotonView m_photonView = null;

    public static LayerMask _inLayer = -1;
    public static LayerMask _outLayer = -1;
    public static LayerMask _doorLayer = -1;
    public static LayerMask _wallLayer = -1;

    protected const float c_inDoorPosZ = 14;
    protected const float c_doorPosZ = 9;
    protected const float c_outDoorPosZ = -1;

    protected GameObject _doorObject = null;
    protected Door _doorScript = null;

    private static bool _isConfirmNullRef = false;

    protected Rigidbody2D _rigidbody2D
    {
        get
        {
            if (!m_rigidbody2D)
            {
                Debug.LogError($"{gameObject} Object doesn't have rigidbody component");
            }
            return m_rigidbody2D;
        }
        set
        {
            m_rigidbody2D = value;
        }
    }

    protected Collider2D _collider2D
    {
        get
        {
            if (!m_collider2D)
            {
                Debug.LogError($"{gameObject} Object doesn't have collider component");
            }
            return m_collider2D;
        }
        set
        {
            m_collider2D = value;
        }
    }

    protected SpriteRenderer _spriteRenderer
    {
        get
        {
            if (!m_spriteRenderer)
            {
                Debug.LogError($"{gameObject} Object doesn't have SpriteRenderer component");
            }
            return m_spriteRenderer;
        }
        set
        {
            m_spriteRenderer = value;
        }
    }

    protected PhotonView _photonView
    {
        get
        {
            if (!m_photonView)
            {
                Debug.LogError($"{gameObject} Object doesn't have photonView component");
            }
            return m_photonView;
        }
        set
        {
            m_photonView = value;
        }
    }

    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        m_photonView = GetComponent<PhotonView>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        _inLayer = LayerMask.NameToLayer("In");
        _outLayer = LayerMask.NameToLayer("Out");
        _doorLayer = LayerMask.NameToLayer("Door");
        _wallLayer = LayerMask.NameToLayer("Wall");

        Init();
        ConfirmNullReference();
    }

    protected virtual void Init() { }


    private void ConfirmNullReference()
    {
        if (_isConfirmNullRef)
        {
            return;
        }
        else
        {
            _isConfirmNullRef = true;
        }

        if (_inLayer < 0)
        {
            Debug.LogError("InDoor Layer is not assigned");
        }
        if (_outLayer < 0)
        {
            Debug.LogError("OutDoor Layer is not assigned");
        }
        if (_doorLayer < 0)
        {
            Debug.LogError("Door Layer is not assigned");
        }
    }

    protected void ChangeLocationStatus(LocationStatus locationStatus)
    {
        Vector3 _pos = transform.position;
        switch (locationStatus)
        {
            case LocationStatus.Out:
                _pos.z = c_outDoorPosZ;
                break;
            case LocationStatus.In:
                _pos.z = c_inDoorPosZ;
                break;
            case LocationStatus.Door:
                _pos.z = c_doorPosZ;
                break;
            default:
                break;
        }
        Debug.Log("ChangeLocationStatus: " + locationStatus);
        transform.position = _pos;
        _locationStatus = locationStatus;
    }
}
