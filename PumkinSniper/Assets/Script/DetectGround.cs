using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    [SerializeField] private Player _player;

    private void Start()
    {
        _player = transform.parent.GetComponent<Player>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _player.Materialize();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _player.Penetrate();
    }
}
