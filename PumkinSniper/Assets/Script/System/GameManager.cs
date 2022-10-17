using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance = null;
    public CharacterBase _character = null;

    public Transform _bulletStorage = null;

    public List<Bullet> _pistolBullets = null;
    public List<Bullet> _machineGunBullets = null;
    public List<Bullet> _shotGunBullets = null;

    [Header("Layer")]
    public LayerMask _inLayer = -1;    //In
    public LayerMask _outLayer = -1;   //Out
    public LayerMask _doorLayer = -1;  //Door
    public LayerMask _wallLayer = -1;  //Wall

    [Header("Tag")]
    public string _playerString = "Player";
    public string _bottomString = "Bottom";
    public string _groundString = "Ground";

    [SerializeField] private Map _map = null;
    

    
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


        _bulletStorage = GameObject.Find("BulletStorage").transform;
        LoadBullets(_bulletStorage.Find("PistolStorage"), ref _pistolBullets);
        LoadBullets(_bulletStorage.Find("MachineGunStorage"), ref _machineGunBullets);
        LoadBullets(_bulletStorage.Find("ShotGunStorage"), ref _shotGunBullets);

        _map.InitGameSetting();
    }

    private Bullet[] bullets;
    private void LoadBullets(Transform storage, ref List<Bullet> bulletsList)
    {
        bullets = storage.GetComponentsInChildren<Bullet>();
        foreach (Bullet bullet in bullets)
        {
            bulletsList.Add(bullet);
        }
    }

    public void RenewMap()
    {
        _map.RenewMap();
    }
}
