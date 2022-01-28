using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SheepPack : MonoBehaviour
{
    public List<Sheep> m_sheepList;

    public float m_roamRangeMin = 5f;
    public float m_roamRangeMax = 10f;

    public void Awake()
    {
        m_sheepList = new List<Sheep>(GetComponentsInChildren<Sheep>());
    }

    // Start is called before the first frame update
    void Start()
    {
        
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
    }

    private void GenerateRoamLocation(Sheep sheep)
    {
        Vector3 direction = Random.insideUnitSphere;
        direction.y = 0;

        float dist = Random.Range(m_roamRangeMin, m_roamRangeMax);

        sheep.SetTarget(GetAveragePosition() + direction.normalized * dist);
    }

    public Vector3 GetAveragePosition()
    {
        Vector3 pos = Vector3.zero;
        foreach (var item in m_sheepList)
        {
            pos += item.transform.position;
        }

        return pos / m_sheepList.Count;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 pos = GetAveragePosition();
        Gizmos.DrawSphere(pos, 0.25f);

        Handles.color = Color.green;
        Handles.DrawWireDisc(pos, Vector3.up, m_roamRangeMin);

        Handles.color = Color.red;
        Handles.DrawWireDisc(pos, Vector3.up, m_roamRangeMax);
    }
}
