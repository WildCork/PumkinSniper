using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance = null;
    public CharacterBase _character = null;
    
    [Header("Layer")]
    public LayerMask _inLayer = -1;    //In
    public LayerMask _outLayer = -1;   //Out
    public LayerMask _doorLayer = -1;  //Door
    public LayerMask _wallLayer = -1;  //Wall

    [Header("Tag")]
    public string _playerString = "Player";
    public string _bottomString = "Bottom";
    public string _groundString = "Ground";

    [SerializeField] private InputController _inputController = null;
    [SerializeField] private Map _map = null;

    /*
     * tag : Ground Bottom
     * layer : In Out Door Wall
     */
    private void Awake()
    {
        if (s_instance)
        {
            Destroy(this);
        }
        s_instance = this;
        InitGameSetting();
        Screen.SetResolution(1920, 1080, false);
    }

    private void InitGameSetting()
    {
        _inLayer = LayerMask.NameToLayer("In");
        _outLayer = LayerMask.NameToLayer("Out");
        _doorLayer = LayerMask.NameToLayer("Door");
        _wallLayer = LayerMask.NameToLayer("Wall");


        _map.InitGameSetting();
    }

    public void RenewMap()
    {
        _map.RenewMap();
    }
}
