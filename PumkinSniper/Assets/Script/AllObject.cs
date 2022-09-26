using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AllObject : MonoBehaviourPunCallbacks
{
    [SerializeField] private SpriteRenderer m_spriteRenderer = null;
    [SerializeField] private Rigidbody2D m_rigidbody2D = null;
    [SerializeField] private Collider2D m_collider2D = null;
    [SerializeField] private PhotonView m_photonView = null;

    [SerializeField] protected bool _isIndoors = false;

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
    }

    private void OnTriggerEnter2D(Collider2D collision) //TODO: check IndoorOutdoor
    {

    }

    private void OnTriggerExit2D(Collider2D collision) //TODO: check IndoorOutdoor
    {

    }
}
