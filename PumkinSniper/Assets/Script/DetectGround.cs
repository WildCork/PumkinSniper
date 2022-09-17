using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectGround : MonoBehaviour
{
    [SerializeField] private CharacterBase _characterBase = null;

    private void Awake()
    {
        _characterBase = transform.parent.GetComponent<CharacterBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        _characterBase.Materialize();
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        _characterBase.Penetrate();
    }
}
