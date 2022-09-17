using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CircleCollider2D _circleCollider2D;
    [SerializeField] private bool _isDetectGround = false;
    // Start is called before the first frame update
    void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0)
        {
            Vector2 preVelocity = _rigidBody.velocity;
            preVelocity.x = Input.GetAxis("Horizontal") * 3;
            _rigidBody.velocity = preVelocity;
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rigidBody.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
        }
    }

    public void Penetrate()
    {
        Debug.Log("Penetrate");
        _circleCollider2D.enabled = false;
    }

    public void Materialize()
    {
        Debug.Log("Maerialize");
        _circleCollider2D.enabled = true;
    }
}
