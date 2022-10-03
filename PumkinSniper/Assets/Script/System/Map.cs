using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform _allMap = null;
    [SerializeField] private Transform _covers = null;
    [SerializeField] private Transform _walls = null;
    [SerializeField] private Transform _doors = null;
    [SerializeField] private Tilemap[] _allMapTilemaps;
    [SerializeField] private Tilemap[] _inCoverTilemaps;
    [SerializeField] private Tilemap[] _outCoverTilemaps;

    [SerializeField] private WaitForSeconds _renewSeconds = new WaitForSeconds(0.05f);
    [SerializeField] private CharacterBase _characterBase = null;

    [SerializeField] private float _alphaChangeValue = 0.1f;

    private Color _color = Color.clear;

    // Start is called before the first frame update
    private void Awake()
    {
        _characterBase = GameManager.s_instance._character;

        Define(ref _allMap, transform.Find("AllMap"));
        Define(ref _covers,transform.Find( "Covers"));
        Define(ref _walls, transform.Find("Walls"));
        Define(ref _doors, transform.Find("Doors"));

        Define(ref _allMapTilemaps ,_allMap.GetComponentsInChildren<Tilemap>());
        Define(ref _inCoverTilemaps,_covers.Find("InCover").GetComponentsInChildren<Tilemap>());
        Define(ref _outCoverTilemaps,_covers.Find("OutCover").GetComponentsInChildren<Tilemap>());
    }

    private void Start()
    {
        StartCoroutine(RenewMap());
    }

    private void Define<T>(ref T t, T define)
    {
        if ((t = define) == null)
        {
            Debug.LogError($"This {define} is null!!");
        }
    }

    IEnumerator RenewMap()
    {
        while (true)
        {
            switch (_characterBase._locationStatus)
            {
                case AllObject.LocationStatus.In:
                    RenewColor(RenewColorType.Transparent, ref _inCoverTilemaps);
                    RenewColor(RenewColorType.Opaque, ref _outCoverTilemaps);
                    break;
                case AllObject.LocationStatus.Out:
                    RenewColor(RenewColorType.Opaque, ref _inCoverTilemaps);
                    RenewColor(RenewColorType.Transparent, ref _outCoverTilemaps);
                    break;
                case AllObject.LocationStatus.Door:
                    RenewColor(RenewColorType.Transparent, ref _inCoverTilemaps);
                    RenewColor(RenewColorType.Transparent, ref _outCoverTilemaps);
                    break;
                default:
                    break;
            }
            yield return _renewSeconds;
        }
    }

    private enum RenewColorType { Opaque, Transparent }
    private void RenewColor(RenewColorType renewType, ref Tilemap[] tiles)
    {
        switch (renewType)
        {
            case RenewColorType.Opaque:
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i].color.a < 1)
                    {
                        _color = tiles[i].color;
                        _color.a += _alphaChangeValue;
                        tiles[i].color = _color;
                    }
                }
                break;
            case RenewColorType.Transparent:
                for (int i = 0; i < tiles.Length; i++)
                {
                    if (tiles[i].color.a > 0)
                    {
                        _color = tiles[i].color;
                        _color.a -= _alphaChangeValue;
                        tiles[i].color = _color;
                    }
                }
                break;
            default:
                break;
        }
    }
}
