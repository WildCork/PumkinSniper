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

    private static bool _isConfirmNullRef = false;

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

    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        m_photonView = GetComponent<PhotonView>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        ConfirmNullReference();
    }

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
    }

}
