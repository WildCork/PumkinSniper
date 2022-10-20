using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager = null;
    public CharacterBase _character = null;

    public Dictionary<BulletKind, List<Bullet>> _bulletStorage = new ();
    public Dictionary<BulletKind, Transform> _storageTransform = new ();

    [Header("Layer")]
    public LayerMask _inLayer = -1;    //In
    public LayerMask _outLayer = -1;   //Out
    public LayerMask _doorLayer = -1;  //Door
    public LayerMask _wallLayer = -1;  //Wall
    public LayerMask _playerLayer = -1;  //Player

    [Header("Tag")]
    public string _playerString = "Player";
    public string _bottomString = "Bottom";
    public string _groundString = "Ground";

    [SerializeField] private Map _map = null;
    

    
    private void Awake()
    {
        if (gameManager)
        {
            Destroy(this);
        }
        gameManager = this;
        InitGameSetting();
        Screen.SetResolution(1920, 1080, false);
    }

    private void InitGameSetting()
    {
        _inLayer = LayerMask.NameToLayer("In");
        _outLayer = LayerMask.NameToLayer("Out");
        _doorLayer = LayerMask.NameToLayer("Door");
        _wallLayer = LayerMask.NameToLayer("Wall");
        _playerLayer = LayerMask.NameToLayer("Player");

        Transform storage = GameObject.Find("BulletStorage").transform;
        LoadBullets(storage.Find("PistolStorage"), BulletKind.Pistol);
        LoadBullets(storage.Find("MachineGunStorage"), BulletKind.Machinegun);
        LoadBullets(storage.Find("ShotGunStorage"), BulletKind.Shotgun);

        _map.InitGameSetting();
    }

    private Bullet[] bullets;
    private void LoadBullets(Transform storage, BulletKind bulletKind)
    {
        _storageTransform[bulletKind] = storage;
        bullets = storage.GetComponentsInChildren<Bullet>();
        _bulletStorage.Add(bulletKind, new());
        foreach (Bullet bullet in bullets)
        {
            bullet._bulletKind = bulletKind;
            _bulletStorage[bulletKind].Add(bullet);
        }

        if (bullets[0]._maxLifeTime / bullets[0]._shootDelayTime > bullets.Length)
        {
            Debug.LogError($"{bulletKind} Storage has not enough bullets!!");
        }
    }

    public void RenewMap()
    {
        _map.RenewMap();
    }
}
