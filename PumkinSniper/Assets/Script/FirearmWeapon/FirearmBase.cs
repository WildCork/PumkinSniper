using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirearmBase : MonoBehaviour
{
    [SerializeField] protected int _bulletCnt = 0;
    [SerializeField] protected int _maxBulletCnt = 0;
    [SerializeField] protected float _shootDelay = 0;
    [SerializeField] protected float _shootSpeed = 0;

    protected int Reload(int cnt)
    {
        _bulletCnt += cnt;
        int _remainCnt = _bulletCnt - _maxBulletCnt;
        if (_bulletCnt > _maxBulletCnt)
        {
            _bulletCnt = _maxBulletCnt;
        }
        else
        {
            _remainCnt = 0;
        }
        return _remainCnt;
    }
    protected void Shoot()
    {

    }
}
