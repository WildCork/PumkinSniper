using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController s_instance = null;

    private void Awake()
    {
        if (s_instance)
        {
            Destroy(this);
        }
        s_instance = this;
    }

    public float _horizontalInput = 0;
    public bool _descendInput = false;
    public bool _tabInput = false;

    public bool _jumpDownInput = false;
    public bool _jumpUpInput = false;
    public bool _shootInput = false;
    public bool _runInput = false;
    public bool _reloadInput = false;
    public bool _getItemInput = false;
    public bool _enterInput = false;

    void Update()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _descendInput = Input.GetKeyDown(KeyCode.S);
        _tabInput = Input.GetKey(KeyCode.Tab);

        _shootInput = Input.GetKey(KeyCode.J);
        _jumpUpInput = Input.GetKeyUp(KeyCode.K);
        _jumpDownInput = Input.GetKeyDown(KeyCode.K);
        _runInput = Input.GetKey(KeyCode.L);

        _reloadInput = Input.GetKeyUp(KeyCode.R);
        _getItemInput = Input.GetKeyDown(KeyCode.F);
    }
}
