using AudioSystem.Agents;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SheepPack : MonoBehaviour
{
    public List<Sheep> m_sheepList;

    public float m_roamRangeMin = 5f;
    public float m_roamRangeMax = 10f;
    public float m_soundDelayPerSheep = -1.5f;
    public float m_soundDelay = 10.0f;

    public float m_timeTillNextSound = 0.0f;

    public int m_maxSheep;
    public GameObject[] m_sheepPrefabs;
    public GameObject[] m_wolfPrefabs;

    private MultiAudioAgent m_soundAgent;
    public void Awake()
    {
        m_sheepList = new List<Sheep>();
        m_soundAgent = GetComponent<MultiAudioAgent>();
        float delay = m_soundDelay + m_soundDelayPerSheep * m_sheepList.Count;
        m_timeTillNextSound = Random.Range(delay * 0.85f, delay * 1.25f);
    }

    public int AddSheep(int count)
    {
        int before = m_sheepList.Count;
        for (int i = 0; i < count; i++)
        {
            Vector3 loc = Random.insideUnitSphere;
            loc.y = 0;
            loc = transform.position + loc.normalized * Random.Range(m_roamRangeMin, m_roamRangeMax);

            if (Physics.OverlapSphere(loc, 1.5f, 1 << LayerMask.NameToLayer("Sheep")).Length > 0)
            {
                i--;
            }
            else
            {
                GameObject nextSheep = Instantiate(m_sheepPrefabs[Random.Range(0, m_sheepPrefabs.Length)], transform);
                nextSheep.transform.position = loc;
                nextSheep.transform.eulerAngles = new Vector3(0, Random.Range(0f, 360f), 0);
                m_sheepList.Add(nextSheep.GetComponent<Sheep>());
            }
        }

        return m_sheepList.Count - before;
    }

    public Wolf AddWolf()
    {
        int limit = 10;
        while(limit > 0)
        {
            Vector3 loc = Random.insideUnitSphere;
            loc.y = 0;
            loc = transform.position + loc.normalized * Random.Range(m_roamRangeMin, m_roamRangeMax);

            if (Physics.OverlapSphere(loc, 1.5f, 1 << LayerMask.NameToLayer("Sheep")).Length > 0)
            {
                //Try again
                limit--;
            }
            else
            {
                GameObject wolf = Instantiate(m_wolfPrefabs[Random.Range(0, m_wolfPrefabs.Length)], loc, Quaternion.Euler(new Vector3(0, Random.Range(0f, 360f), 0)));
                return wolf.GetComponent<Wolf>();
            }
        }
        return null;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var item in m_sheepList)
        {
            if(item.m_isWaitingForDestination)
            {
                GenerateRoamLocation(item);
            }
        }

        if(m_sheepList.Count == 0)
        {
            (GameManager.Instance as GameManager).RemovePack(this);
            Destroy(gameObject);
        }

        m_timeTillNextSound -= Time.deltaTime;
        if (m_timeTillNextSound <= 0f)
        {
            m_soundAgent.Play("SheepIdle", false, Random.Range(0.85f, 1.25f));

            float delay = m_soundDelay + m_soundDelayPerSheep * m_sheepList.Count;
            m_timeTillNextSound = Random.Range(delay * 0.85f, delay * 1.25f);
        }
    }

    public void GenerateRoamLocation(Sheep sheep)
    {
        Vector3 direction = Random.insideUnitSphere;
        direction.y = 0;

        float dist = Random.Range(m_roamRangeMin, m_roamRangeMax);

        sheep.SetTarget(GetAveragePosition() + direction.normalized * dist);
    }

    public Sheep GetClosestSheep(Vector3 pos)
    {
        Sheep closest = null;

        float dist = float.MaxValue;
        foreach (var sheep in m_sheepList)
        {
            float newDist = Vector3.Distance(pos, sheep.transform.position);
            if (newDist < dist)
            {
                closest = sheep;
                dist = newDist;
            }
        }

        return closest;
    }

    public Vector3 GetAveragePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach (var item in m_sheepList)
        {
            pos += item.transform.position;
        }

        if(m_sheepList.Count == 0)
            return transform.position;

        return pos / m_sheepList.Count;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 pos = (m_sheepList.Count > 0) ? GetAveragePosition() : transform.position;
        Gizmos.DrawSphere(pos, 0.5f);

        RaycastHit hit;
        Physics.Raycast(pos, Vector3.down * transform.position.y, out hit, 1 << LayerMask.NameToLayer("Ground"));

        Handles.color = Color.green;
        Handles.DrawWireDisc(pos, hit.normal, m_roamRangeMin);

        Handles.color = Color.red;
        Handles.DrawWireDisc(pos, hit.normal, m_roamRangeMax);
    }

    public void Destroy(Sheep sheep)
    {
        m_soundAgent.Play("SheepDeath", false, Random.Range(0.85f, 1.25f));
        m_sheepList.Remove(sheep);
        Destroy(sheep.gameObject);

        if (m_sheepList.Count == 0)
        {
            GameManager.Instance.RemovePack(this);
            Destroy(gameObject);
        }
    }
}
