using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController s_instance = null;

    private void Awake()
    {
        if (!s_instance)
        {
            s_instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public float _horizontalInput = 0;
    public bool _jumpDownInput = false;
    public bool _jumpUpInput = false;
    public bool _goDownInput = false;

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _jumpDownInput = Input.GetKeyDown(KeyCode.Space);
        _jumpUpInput = Input.GetKeyUp(KeyCode.Space);
        _goDownInput = Input.GetKeyDown(KeyCode.DownArrow);
    }
}
