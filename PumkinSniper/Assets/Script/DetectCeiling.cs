using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectCeiling : MonoBehaviour
{
    [SerializeField] private CharacterBase _characterBase = null;

    private void Awake()
    {
        _characterBase = transform.parent.GetComponent<CharacterBase>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (_characterBase._locationStatus)
        {
            case AllObject.LocationStatus.Out:
                if (collision.gameObject.layer == AllObject._outLayer)
                {
                    _characterBase.Penetrate();
                }
                break;
            case AllObject.LocationStatus.In:
                if (collision.gameObject.layer == AllObject._inLayer)
                {
                    _characterBase.Penetrate();
                }
                break;
            default:
                break;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (_characterBase._locationStatus)
        {
            case AllObject.LocationStatus.Out:
                if (collision.gameObject.layer == AllObject._outLayer)
                {
                    _characterBase.Materialize();
                }
                break;
            case AllObject.LocationStatus.In:
                if (collision.gameObject.layer == AllObject._inLayer)
                {
                    _characterBase.Materialize();
                }
                break;
            default:
                break;
        }
    }
}
