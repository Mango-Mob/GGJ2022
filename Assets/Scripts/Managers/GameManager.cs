using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public GameObject m_dogPrefab;

    public List<SheepPack> m_packOfSheep { get; private set; }

    public List<Wolf> m_wolfList { get; private set; }

    public Dog m_dog { get; set; }

    public Camera m_playerCamera { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        m_packOfSheep = new List<SheepPack>(FindObjectsOfType<SheepPack>());
        m_wolfList = new List<Wolf>(FindObjectsOfType<Wolf>());
        m_playerCamera = FindObjectOfType<Camera>();
    }

    protected virtual void Update()
    {
        
    }

    public SheepPack GetNearestPack(Vector3 position)
    {
        SheepPack closest = null;

        float dist = float.MaxValue;
        foreach (var item in m_packOfSheep)
        {
            float newDist = Vector3.Distance(position, item.GetAveragePosition());
            if(newDist < dist)
            {
                closest = item;
                dist = newDist;
            }
        }

        return closest;
    }

    public void NotifyAnimalsOfShot(Vector3 hitLoc)
    {

    }

    public int GetSheepCount()
    {
        int count = 0;
        foreach (var item in m_packOfSheep)
        {
            count += item.m_sheepList.Count;
        }
        return count;
    }

    public void RemovePack(SheepPack pack)
    {
        if(pack != null)
            m_packOfSheep.Remove(pack);
    }
}
