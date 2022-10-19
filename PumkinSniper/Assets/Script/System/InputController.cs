using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InputController : MonoBehaviour
{
    public static InputController inputController = null;

    private void Awake()
    {
        if (inputController)
        {
            Destroy(this);
        }
        inputController = this;
    }

    public float _horizontal = 0;
    public bool _descend = false;

    public bool _shootUp = false;
    public bool _shootDown = false;
    public bool _grenadeDown = false;
    public bool _jumpDown = false;
    public bool _jumpUp = false;

    //public bool _run = false;
    public bool _walk = false;

    public bool _tab = false;

    private  KeyCode _tabCode = KeyCode.Tab;
    private  string _horizontalString = "Horizontal";
    private  KeyCode _descendCode = KeyCode.DownArrow;

    private  KeyCode _shootCode = KeyCode.Z;
    private  KeyCode _jumpCode = KeyCode.X;
    private  KeyCode _grenadeCode = KeyCode.C;
    //private  KeyCode _runCode = KeyCode.LeftShift;

    private  KeyCode _walkCode = KeyCode.LeftControl;

    private float _canJumpUpTime = 0f;
    private float _canJumpDownTime = 0f;
    private float _canGrenadeTime = 0f;
    private float _canDescendTime = 0f;

    private void Update()
    {
        _horizontal = Input.GetAxisRaw(_horizontalString);
        _shootUp = Input.GetKeyUp(_shootCode);
        _shootDown = Input.GetKeyDown(_shootCode);
        _walk = Input.GetKey(_walkCode);
        _tab = Input.GetKey(_tabCode);

        if (!_grenadeDown)
            _grenadeDown = Input.GetKeyDown(_grenadeCode);
        if (!_descend)
            _descend = Input.GetKeyDown(_descendCode);
        if (!_jumpUp)
            _jumpUp = Input.GetKeyUp(_jumpCode);
        if (!_jumpDown)
            _jumpDown = Input.GetKeyDown(_jumpCode);

        ConfirmCancel(ref _grenadeDown, ref _canGrenadeTime);
        ConfirmCancel(ref _descend, ref _canDescendTime);
        ConfirmCancel(ref _jumpUp, ref _canJumpUpTime);
        ConfirmCancel(ref _jumpDown, ref _canJumpDownTime);
    }

    private void ConfirmCancel(ref bool isKey, ref float canTime)
    {
        if(!isKey)
        {
            canTime = 0f;
        }
        else
        {
            canTime += Time.deltaTime;
            if (canTime > Time.fixedDeltaTime * 2)
            {
                isKey = false;
            }
        }
    }
}
