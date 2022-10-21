using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Bullet;
using static Grenade;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManager = null;
    public CharacterBase _character = null;

    public Dictionary<BulletType, List<Bullet>> _bulletStorage = new();
    public Dictionary<BulletType, Vector2> _bulletStorageTransform = new();

    public Dictionary<GrenadeType, List<Grenade>> _grenadeStorage = new();
    public Dictionary<GrenadeType, Vector2> _grenadeStorageTransform = new();


    [Header("Layer")]
    public LayerMask _inLayer = -1;    //In
    public LayerMask _outLayer = -1;   //Out
    public LayerMask _doorLayer = -1;  //Door
    public LayerMask _wallLayer = -1;  //Wall
    public LayerMask _playerLayer = -1;  //Player

    [Header("Tag")]
    public string _playerTag = "Player";
    public string _bottomTag = "Bottom";
    public string _groundTag = "Ground";

    [Header("Name")]
    public string _storageName = "Storage";
    public string _pistolStorageName = "PistolStorage";
    public string _machineGunStorageName = "MachineGunStorage";
    public string _shotGunStorageName = "ShotGunStorage";
    public string _bombStorageName = "BombStorage";

    public string _mapName = "Map";

    [SerializeField] private Map _map = null;
    

    
    private void Start()
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

        Transform storage = GameObject.Find(_storageName).transform;

        LoadBullet(storage.Find(_pistolStorageName), BulletType.Pistol);
        LoadBullet(storage.Find(_machineGunStorageName), BulletType.Machinegun);
        LoadBullet(storage.Find(_shotGunStorageName), BulletType.Shotgun);

        LoadGrenade(storage.Find(_bombStorageName), GrenadeType.Bomb);

        _map = GameObject.Find(_mapName).GetComponent<Map>();
        _map.InitGameSetting();
    }

    private Bullet[] bullets;
    private void LoadBullet(Transform storage, BulletType bulletType)
    {
        _bulletStorageTransform[bulletType] = storage.position;
        bullets = storage.GetComponentsInChildren<Bullet>();
        _bulletStorage.Add(bulletType, new());
        foreach (Bullet bullet in bullets)
        {
            bullet._bulletType = bulletType;
            _bulletStorage[bulletType].Add(bullet);
        }

        if (bullets[0]._maxLifeTime / bullets[0]._shootDelayTime > bullets.Length)
        {
            Debug.LogError($"{bulletType} Storage has not enough bullets!!");
        }
    }

    private Grenade[] grenades;
    private void LoadGrenade(Transform storage, GrenadeType grenadeType)
    {
        _grenadeStorageTransform[grenadeType] = storage.position;
        grenades = storage.GetComponentsInChildren<Grenade>();
        _grenadeStorage.Add(grenadeType, new());
        foreach (Grenade grenade in grenades)
        {
            grenade._grenadeType = grenadeType;
            _grenadeStorage[grenadeType].Add(grenade);
        }
    }
    public void RenewMap()
    {
        _map.RenewMap();
    }
}
