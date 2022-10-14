using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DetectGround : AllObject
{
    private string bottomString = "Bottom";
    private string groundString = "Ground";
    private HashSet<Collider2D> m_Grounds = new HashSet<Collider2D>();
    private CharacterBase _characterBase
    {
        get { return GameManager.s_instance._character; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Vector2.Dot(ContactNormalVec(collision, _characterBase.transform.position), Vector2.up) >= -0.5f)
        {
            return;
        }
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    MakeGround(ref collision, true);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    MakeGround(ref collision, true);
                }
                break;
            case LocationStatus.Door:
                MakeGround(ref collision, true);
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
                    MakeGround(ref collision, false);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    MakeGround(ref collision, false);
                }
                break;
            case LocationStatus.Door:
                break;
            default:
                break;
        }
    }

    private void MakeGround(ref Collider2D collision, bool isReal) //isReal=True 지형 실체화 isReal=False 지형 투영화
    {
        if (isReal)
        {
            collision.isTrigger = false;
            m_Grounds.Add(collision);
            _characterBase.RefreshOnGround(true);
        }
        else
        {
            if (!collision.gameObject.CompareTag(bottomString))
            {
                collision.isTrigger = true;
            }
            m_Grounds.Remove(collision);
            if (m_Grounds.Count == 0)
            {
                _characterBase.RefreshOnGround(false);
            }
        }
    }

    public void Descend()
    {
        foreach (Collider2D collision in m_Grounds)
        {
            if (collision.CompareTag(bottomString))
            {
                collision.isTrigger = false;
            }
            else
            {
                collision.isTrigger = true;
            }
        }
    }
}
