using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DetectCeiling : AllObject
{
    [SerializeField] private CharacterBase _characterBase = null;
    private Vector2 _dir;

    private void Start()
    {
        _characterBase = GameManager.s_instance._character;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //TODO: �ǳ� ���� �浹 ó�� �̻� �߻� - ���� �ʿ�
        _dir = collision.transform.position - _characterBase.transform.position;
        if (Vector2.Dot(_dir.normalized, Vector2.up) > 0)
        {
            //�Ʒ��� �������� ���������� ��� ��� ���� ó��
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
    private void OnTriggerExit2D(Collider2D collision)
    {
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.Out:
                if (collision.gameObject.layer == _outLayer)
                {
                    _characterBase.Materialize();
                }
                break;
            case LocationStatus.In:
                if (collision.gameObject.layer == _inLayer)
                {
                    _characterBase.Materialize();
                }
                break;
            default:
                break;
        }
    }
}
