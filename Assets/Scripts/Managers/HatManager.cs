using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HatManager : Singleton<HatManager>
{
    public GameObject[] m_hatPrefabs;

    [Range(0, 100)]
    public float m_probOfHat;

    public GameObject GetAHat(Transform location)
    {
        int limit = 10000;

        if(Random.Range(0f, limit) < (m_probOfHat/100f) * limit && m_hatPrefabs.Length > 0)
        {
            int select = Random.Range(0, m_hatPrefabs.Length);
            return GameObject.Instantiate(m_hatPrefabs[select], location);

        }

        return null;
    }
}
