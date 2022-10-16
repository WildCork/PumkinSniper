using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeBase : MonoBehaviour
{
    [SerializeField] private int _damage = 0;
    [SerializeField] private float _attackDelay = 0;
    public enum MeleeKind { Knife, Bat}
    protected void Hit()
    {
        
    }
}
