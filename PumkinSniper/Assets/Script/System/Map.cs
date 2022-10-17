using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static ObjectBase;

public class Map : MonoBehaviour
{
    [Header("String")]
    private string _coversString = "Covers";
    private string _wallString = "Walls";
    private string _doorsString = "Doors";

    private string _inMapString = "InMap";
    private string _outMapString = "OutMap";
    private string _onInString =  "OnIn";
    private string _onOutString = "OnOut";

    private string  _coverString = "Cover";
    private string  _inCoverString = "InCover";
    private string _outCoverString = "OutCover";

    private Transform _covers = null;
    private Transform _walls = null;
    private Transform _doors = null;
    private Transform _inMap = null;
    private Transform _outMap = null;

    [Header("Collider And Sprites")]

    [SerializeField] private HashSet<SpriteRenderer> _inCovers = new();
    [SerializeField] private HashSet<SpriteRenderer> _outCovers = new();
    [SerializeField] private SpriteRenderer[] _inGrounds = null;
    [SerializeField] private SpriteRenderer[] _outGroundsOnOut = null;
    [SerializeField] private SpriteRenderer[] _outGroundsOnIn = null;
    [SerializeField] private Collider2D[] _wallColliders = null;

    [Header("Values")]
    [SerializeField] private WaitForSeconds _renewSeconds = null;
    [SerializeField] private float _renewTime = 0.05f;
    [SerializeField] private float _alphaChangeValue = 0.1f;

    private CharacterBase _characterBase
    {
        get { return GameManager.s_instance._character; }
    }

    public void InitGameSetting()
    {
        _covers = transform.Find(_coversString);
        _walls = transform.Find(_wallString);
        _inMap = transform.Find(_inMapString);
        _outMap = transform.Find(_outMapString);
        _doors = transform.Find(_doorsString);

        _renewSeconds = new WaitForSeconds(_renewTime);

        InitCovers();
        InitMaps();
        InitWalls();
        InitDoors();
    }

    private Color _color = Color.clear;

    private void InitCovers()
    {
        foreach (Transform cover in _covers)
        {
            if (cover.name.Contains(_inCoverString))
            {
                _inCovers.Add(cover.Find(_coverString).GetComponent<SpriteRenderer>());
            }
            else if (cover.name.Contains(_outCoverString))
            {
                _outCovers.Add(cover.Find(_coverString).GetComponent<SpriteRenderer>());
            }
        }
    }

    private void InitMaps()
    {
        Transform[] maps = GetComponentsInChildren<Transform>();
        foreach (Transform map in maps)
        {
            if (map.name.Contains(GameManager.s_instance._bottomString))
            {
                map.tag = GameManager.s_instance._bottomString;
            }
            else if (map.name.Contains(GameManager.s_instance._groundString))
            {
                map.tag = GameManager.s_instance._groundString;
            }
        }

        Transform[] inMaps = _inMap.GetComponentsInChildren<Transform>();
        foreach (Transform map in inMaps)
        {
            map.gameObject.layer = GameManager.s_instance._inLayer;
        }
        Transform[] outMaps = _outMap.GetComponentsInChildren<Transform>();
        foreach (Transform map in outMaps)
        {
            map.gameObject.layer = GameManager.s_instance._outLayer;
        }

        _inGrounds = _inMap.GetComponentsInChildren<SpriteRenderer>();
        _outGroundsOnOut = _outMap.Find(_onOutString).GetComponentsInChildren<SpriteRenderer>();
        _outGroundsOnIn = _outMap.Find(_onInString).GetComponentsInChildren<SpriteRenderer>();
    }

    private void InitWalls()
    {
        Transform[] walls = _walls.GetComponentsInChildren<Transform>();
        foreach (Transform wall in walls)
        {
            wall.gameObject.layer = GameManager.s_instance._wallLayer;
        }
        _wallColliders = _walls.GetComponentsInChildren<Collider2D>();
    }

    private void InitDoors()
    {
        Transform[] doors = _doors.GetComponentsInChildren<Transform>();
        foreach (Transform door in doors)
        {
            door.gameObject.layer = GameManager.s_instance._doorLayer;
        }
    }

    public void RenewMap()
    {
        StopAllCoroutines();
        StartCoroutine(RefreshMap());
    }


    private bool isEnd = false;
    public IEnumerator RefreshMap()
    {
        switch (_characterBase._locationStatus)
        {
            case LocationStatus.In:
                MakeWall(true);
                break;
            case LocationStatus.Out:
                MakeWall(false);
                break;
            case LocationStatus.Door:
                MakeWall(true);
                break;
            default:
                break;
        }
        isEnd = false;
        while (!isEnd)
        {
            isEnd = true;
            switch (_characterBase._locationStatus)
            {
                case LocationStatus.In:
                    RefreshCover(ref _inCovers, false);
                    RefreshCover(ref _outCovers, true);
                    RefreshGround(ref _outGroundsOnOut, false);
                    RefreshGround(ref _outGroundsOnIn, false);
                    break;
                case LocationStatus.Out:
                    RefreshCover(ref _inCovers, true);
                    RefreshCover(ref _outCovers, false);
                    RefreshGround(ref _outGroundsOnOut, true);
                    RefreshGround(ref _outGroundsOnIn, true);
                    break;
                case LocationStatus.Door:
                    RefreshCover(ref _inCovers, false);
                    RefreshCover(ref _outCovers, false);
                    RefreshGround(ref _outGroundsOnOut, true);
                    RefreshGround(ref _outGroundsOnIn, false);
                    break;
                default:
                    break;
            }
            yield return _renewSeconds;
        }
    }

    private void MakeWall(bool condition)
    {
        foreach (Collider2D collider in _wallColliders)
        {
            collider.isTrigger = !condition; 
        }
    }

    private void RefreshCover(ref HashSet<SpriteRenderer> spriteSet, bool isOpaque)
    {
        if (isOpaque)
        {
            foreach (SpriteRenderer renderer in spriteSet)
            {
                if(renderer.color.a < 1)
                {
                    _color = renderer.color;
                    _color.a += _alphaChangeValue;
                    renderer.color = _color;
                    isEnd = false;
                }
            }
        }
        else
        {
            foreach (SpriteRenderer renderer in spriteSet)
            {
                if (renderer.color.a > 0)
                {
                    _color = renderer.color;
                    _color.a -= _alphaChangeValue;
                    renderer.color = _color;
                    isEnd = false;
                }
            }
        }
    }

    private void RefreshGround(ref SpriteRenderer[] grounds, bool isOpaque)
    {
        if (isOpaque)
        {
            foreach (SpriteRenderer renderer in grounds)
            {
                if (renderer.color.a < 1)
                {
                    _color = renderer.color;
                    _color.a += _alphaChangeValue;
                    renderer.color = _color;
                    isEnd = false;
                }
            }
        }
        else
        {
            foreach (SpriteRenderer renderer in grounds)
            {
                if (renderer.color.a > 0)
                {
                    _color = renderer.color;
                    _color.a -= _alphaChangeValue;
                    renderer.color = _color;
                    isEnd = false;
                }
            }
        }
    }
}
