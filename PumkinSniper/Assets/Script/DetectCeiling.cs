using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectCeiling : AllObject
{
    [SerializeField] 
    private CharacterBase _characterBase
    {
        get { return GameManager.s_instance._character; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Vector2.Dot(ContactNormalVec(collision, _characterBase.transform.position), Vector2.up) < 0)
        {
            return;
        }
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    _characterBase.Penetrate();
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    _characterBase.Penetrate();
                }
                break;
            default:
                break;
        }
    }
}
