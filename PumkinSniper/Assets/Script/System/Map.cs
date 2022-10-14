using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static AllObject;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform _covers = null;
    [SerializeField] private HashSet<SpriteRenderer> _inCovers = new();
    [SerializeField] private HashSet<SpriteRenderer> _outCovers = new();

    [SerializeField] private WaitForSeconds _renewSeconds = new WaitForSeconds(0.05f);

    private CharacterBase _characterBase
    {
        get { return GameManager.s_instance._character; }
    }

    [SerializeField] private float _alphaChangeValue = 0.1f;

    private Color _color = Color.clear;

    // Start is called before the first frame update
    private void Awake()
    {
        _covers = transform.Find("Covers");
        foreach (Transform cover in _covers)
        {
            if (cover.name.Contains("InCover"))
            {
                _inCovers.Add(cover.Find("Cover").gameObject.GetComponent<SpriteRenderer>());
            }
            else if (cover.name.Contains("OutCover"))
            {
                _outCovers.Add(cover.Find("Cover").gameObject.GetComponent<SpriteRenderer>());
            }
        }
    }

    public void RenewMap()
    {
        StopAllCoroutines();
        StartCoroutine(RefreshMap());
    }


    public IEnumerator RefreshMap()
    {
        bool isEnd1 = false, isEnd2 = false;
        while (!(isEnd1 && isEnd2))
        {
            switch (_characterBase._locationStatus)
            {
                case LocationStatus.In:
                    if (!isEnd1) isEnd1 = RefreshColor(ref _inCovers, false);
                    if (!isEnd2) isEnd2 = RefreshColor(ref _outCovers, true);
                    break;
                case LocationStatus.Out:
                    if (!isEnd1) isEnd1 = RefreshColor(ref _inCovers, true);
                    if (!isEnd2) isEnd2 = RefreshColor(ref _outCovers, false);
                    break;
                case LocationStatus.Door:
                    if (!isEnd1) isEnd1 = RefreshColor(ref _inCovers, false);
                    if (!isEnd2) isEnd2 = RefreshColor(ref _outCovers, false);
                    break;
                default:
                    break;
            }
            yield return _renewSeconds;
        }
    }

    private bool RefreshColor(ref HashSet<SpriteRenderer> spriteSet, bool isOpaque)
    {
        bool isEnd = true;
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
        return isEnd;
    }
}
