using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectFloor : AllObject
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
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    _characterBase.RefreshOnGround(true);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    _characterBase.RefreshOnGround(true);
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
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    _characterBase.RefreshOnGround(false);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    _characterBase.RefreshOnGround(false);
                }
                break;
            default:
                break;
        }
    }
}
