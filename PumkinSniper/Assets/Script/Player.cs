using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rigidBody;
    [SerializeField] private CircleCollider2D _circleCollider2D;
    [SerializeField] private bool _isOnGround = false;
    // Start is called before the first frame update
    private void Start()
    {
        _rigidBody = GetComponent<Rigidbody2D>();
        _circleCollider2D = GetComponent<CircleCollider2D>();
    }

    private void Update()
    {
        if (_isOnGround)
        {
            if (Input.GetAxis("Horizontal") != 0)
            {
                Vector2 preVelocity = _rigidBody.velocity;
                preVelocity.x = Input.GetAxis("Horizontal") * 3;
                _rigidBody.velocity = preVelocity;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _rigidBody.AddForce(Vector2.up * 12, ForceMode2D.Impulse);
            }
            else if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                Penetrate();
            }
        }
    }

    public void Penetrate()
    {
        _isOnGround = false;
        _circleCollider2D.enabled = false;
    }

    public void Materialize()
    {
        _isOnGround = true;
        _circleCollider2D.enabled = true;
    }
}
