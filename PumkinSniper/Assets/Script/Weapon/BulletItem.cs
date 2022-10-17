using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletItem : ObjectBase
{
    [SerializeField] private FirearmBase.FirearmKind _fireArmKind = FirearmBase.FirearmKind.Machinegun;
    private void Start()
    {
        switch (_fireArmKind)
        {
            case FirearmBase.FirearmKind.Pistol:
                Debug.LogWarning("Pistol is not item");
                break;
            case FirearmBase.FirearmKind.Machinegun:
                _spriteRenderer.color = Color.yellow;
                break;
            case FirearmBase.FirearmKind.Shotgun:
                _spriteRenderer.color = Color.red;
                break;
            default:
                break;
        }

        _rigidbody2D.freezeRotation = true;
    }
}
