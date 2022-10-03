using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager s_instance = null;
    public CharacterBase _character = null;
    private void Awake()
    {
        if (s_instance)
        {
            Destroy(this);
        }
        s_instance = this;

        Screen.SetResolution(1920, 1080, false);
    }

    void Update()
    {
        
    }
}
