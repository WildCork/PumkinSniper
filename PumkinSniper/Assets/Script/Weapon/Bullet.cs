using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Bullet : ObjectBase
{
    [SerializeField] private float _damage;
    [SerializeField] private float _lifeTime;
    [SerializeField] private float _shotSpeed;
    public void Fire()
    {
        gameObject.SetActive(true);
    }

    [PunRPC]
    private void Damage()
    {

    }

    private void Disappear()
    {

    }
}
