using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AllObject : MonoBehaviourPunCallbacks
{
    [SerializeField] private Rigidbody2D m_rigidbody2D = null;
    [SerializeField] private Collider2D m_collider2D = null;
    [SerializeField] private PhotonView _photonView = null;
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


    private void Awake()
    {
        m_rigidbody2D = GetComponent<Rigidbody2D>();
        m_collider2D = GetComponent<Collider2D>();
        _photonView = GetComponent<PhotonView>();
    }

    private void OnTriggerEnter2D(Collider2D collision) //TODO: check IndoorOutdoor
    {

    }

    private void OnTriggerExit2D(Collider2D collision) //TODO: check IndoorOutdoor
    {

    }
}
