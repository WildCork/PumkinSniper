using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameManager;
using static Bullet;

public class Item : ObjectBase
{
    private enum ItemType { Bullet, Health, Grenade }
    private enum SizeType { Small, Large }

    [SerializeField] private ItemType _itemType = ItemType.Bullet;
    [SerializeField] private SizeType _sizeType = SizeType.Small;

    [SerializeField] private BulletType _bulletKind = BulletType.Pistol;
    [SerializeField] private const float c_smallPenaltyRate = 0.8f;
    [SerializeField] private const float c_largePenaltyRate = 0.6f;

    private CharacterBase _character;

    protected override void Hit(Collider2D collision)
    {
        if (collision.gameObject.layer == gameManager._playerLayer)
        {
            _character = collision.gameObject.GetComponent<CharacterBase>();
            switch (_itemType)
            {
                case ItemType.Bullet:
                    Reload(_character);
                    break;
                case ItemType.Health:
                    Heal(_character);
                    break;
                case ItemType.Grenade:
                    break;
                default:
                    break;
            }
            gameObject.SetActive(false);
        }

    }

    private void Reload(CharacterBase character)
    {
        int bulletCnt = 0;
        switch (_sizeType)
        {
            case SizeType.Small:
                switch (_bulletKind)
                {
                    case BulletType.Machinegun:
                        bulletCnt = 100;
                        break;
                    case BulletType.Shotgun:
                        bulletCnt = 20;
                        break;
                    default:
                        bulletCnt = 0;
                        break;
                }
                break;
            case SizeType.Large:
                switch (_bulletKind)
                {
                    case BulletType.Machinegun:
                        bulletCnt = 200;
                        break;
                    case BulletType.Shotgun:
                        bulletCnt = 50;
                        break;
                    default:
                        bulletCnt = 0;
                        break;
                }
                bulletCnt = (int)(bulletCnt * c_largePenaltyRate);
                break;
            default:
                break;
        }
        if (character.currentBulletType == _bulletKind)
        {
            switch (_sizeType)
            {
                case SizeType.Small:
                    bulletCnt = (int)(bulletCnt * c_smallPenaltyRate);
                    break;
                case SizeType.Large:
                    bulletCnt = (int)(bulletCnt * c_largePenaltyRate);
                    break;
                default:
                    break;
            }
            character.BulletCnt += bulletCnt;
        }
        else
        {
            character.currentBulletType = _bulletKind;
            character.BulletCnt = bulletCnt;
        }
    }

    private void Heal(CharacterBase character)
    {
        switch (_sizeType)
        {
            case SizeType.Small:
                character.HP += 20;
                break;
            case SizeType.Large:
                character.HP += 50;
                break;
            default:
                break;
        }
    }
}
