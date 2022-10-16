using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : AllObject
{
    private ParticleSystem m_particleSystem;

    private ParticleSystem _particleSystem
    {
        get
        {
            if (!m_particleSystem)
            {
                m_particleSystem = GetComponent<ParticleSystem>();
            }
            return m_particleSystem;
        }
    }
}
