using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [Tooltip("from OutDoor to InDoor")]
    public Vector2 _doorDirection; //from OutDoor to InDoor
    private void Awake()
    {
        _doorDirection = _doorDirection.normalized;
    }
}
