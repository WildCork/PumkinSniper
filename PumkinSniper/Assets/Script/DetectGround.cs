using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class DetectGround : AllObject
{
    private string bottomString = "Bottom";
    private string groundString = "Ground";
    [SerializeField] private List<Collider2D> m_Grounds = new();
    private CharacterBase _characterBase
    {
        get { return GameManager.s_instance._character; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == GameManager.s_instance._playerString)
        {
            return;
        }
        if (Vector2.Dot(ContactNormalVec(collision, _characterBase.transform.position), Vector2.up) >= -0.5f)
        {
            return;
        }
        if (!m_Grounds.Contains(collision))
        {
            m_Grounds.Add(collision);
        }
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == GameManager.s_instance._outLayer)
                {
                    MakeGround(ref collision, true);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == GameManager.s_instance._inLayer)
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
        if (collision.gameObject.tag == GameManager.s_instance._playerString)
        {
            return;
        }
        m_Grounds.Remove(collision);
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == GameManager.s_instance._outLayer)
                {
                    MakeGround(ref collision, false);
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == GameManager.s_instance._inLayer)
                {
                    MakeGround(ref collision, false);
                }
                break;
            case LocationStatus.Door:
                MakeGround(ref collision, false);
                break;
            default:
                break;
        }
    }

    private void MakeGround(ref Collider2D collision, bool isReal) //isReal=True 지형 실체화 isReal=False 지형 투영화
    {
        if (isReal)
        {
            if (collision.gameObject.layer != GameManager.s_instance._doorLayer)
            {
                collision.isTrigger = false;
                _characterBase.RefreshOnGround(true);
            }
        }
        else
        {
            if (collision.gameObject.layer != GameManager.s_instance._wallLayer &&
                collision.gameObject.CompareTag(bottomString) == false)
            {
                collision.isTrigger = true;
            }
            if (m_Grounds.Count == 0)
            {
                _characterBase.RefreshOnGround(false);
            }
        }
    }

    public void Descend()
    {
        foreach (Collider2D ground in m_Grounds)
        {
            if (!ground.CompareTag(bottomString))
            {
                ground.isTrigger = true;
                m_Grounds.Remove(ground);
                break;
            }
        }
    }

    private Vector2 ContactNormalVec(Collider2D collision, Vector2 pos)
    {
        return (collision.ClosestPoint(pos) - pos).normalized;
    }
}
