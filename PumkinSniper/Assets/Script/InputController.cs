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
    public bool _goDownInput = false;
    public bool _tabInput = false;

    public bool _jumpDownInput = false;
    public bool _shootInput = false;
    public bool _runInput = false;
    public bool _reloadInput = false;
    public bool _getItemInput = false;

    void Update()
    {
        _horizontalInput = Input.GetAxis("Horizontal");
        _goDownInput = Input.GetKeyDown(KeyCode.S);
        _tabInput = Input.GetKey(KeyCode.Tab);

        _shootInput = Input.GetKey(KeyCode.J);
        _jumpDownInput = Input.GetKeyDown(KeyCode.K);
        _runInput = Input.GetKeyDown(KeyCode.L);

        _reloadInput = Input.GetKeyUp(KeyCode.R);
        _getItemInput = Input.GetKeyDown(KeyCode.F);
    }
}
