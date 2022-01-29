using AudioSystem.Agents;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [SerializeField] public GameObject m_dogPrefab;

    public List<SheepPack> m_packOfSheep { get; private set; }

    public int m_sheepSpawnCount = 20;
    public int m_wolfSpawnCount = 3;

    public List<Wolf> m_wolfList { get; private set; }

    public Dog m_dog { get; set; }

    public Camera m_playerCamera { get; private set; }

    public DateTime m_startTime { get; private set; }
	
    public int m_ammoCount = 8;
    MultiAudioAgent m_music;

    protected override void Awake()
    {
        base.Awake();
        m_packOfSheep = new List<SheepPack>(FindObjectsOfType<SheepPack>());

        m_wolfList = new List<Wolf>(FindObjectsOfType<Wolf>());
        m_playerCamera = FindObjectOfType<Camera>();

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Default"), LayerMask.NameToLayer("Projectile"));
    }

    private void Start()
    {
        m_music = GetComponent<MultiAudioAgent>();

        m_music.Play("The Old Country Farm MP3", true);

        m_startTime = DateTime.Now;

        List<SheepPack> spawnOption = new List<SheepPack>(m_packOfSheep);
        int count = 0;

        while (count < m_sheepSpawnCount)
        {
            SheepPack pack = spawnOption[UnityEngine.Random.Range(0, spawnOption.Count)];
            count += pack.AddSheep(Mathf.Min(2, pack.m_maxSheep - pack.m_sheepList.Count, m_sheepSpawnCount - count));

            if (pack.m_sheepList.Count >= pack.m_maxSheep)
            {
                spawnOption.Remove(pack);
            }

            if (spawnOption.Count == 0)
                break;

        }

        spawnOption = new List<SheepPack>(m_packOfSheep);
        count = 0;
        while(count < m_wolfSpawnCount)
        {
            SheepPack pack = spawnOption[UnityEngine.Random.Range(0, spawnOption.Count)];

            if(pack.m_sheepList.Count == 0)
            {
                spawnOption.Remove(pack);
                continue;
            }

            Wolf wolf = pack.AddWolf();
            if(wolf != null)
            {
                count++;
                m_wolfList.Add(wolf);
            }

            if (count >= m_wolfSpawnCount)
                break;

            spawnOption.Remove(pack);
        }
    }

    protected virtual void Update()
    {
        if(m_wolfList.Count == 0)
        {
            //Victory!
            GameOverScreen.SetScene(ScreenState.Victory, "Wolves are defeated", GetSheepCount(), m_startTime);
            LevelManager.Instance.LoadNewLevel("EndScreen");
            return;
        }
        if(m_packOfSheep.Count == 0)
        {
            //Defeat :(
            GameOverScreen.SetScene(ScreenState.Defeat, "Wolves have killed all the sheep", m_wolfList.Count, m_startTime);
            LevelManager.Instance.LoadNewLevel("EndScreen");
            return;
        }
        if(m_ammoCount <= 0)
        {
            //Defeat :(
            GameOverScreen.SetScene(ScreenState.Defeat, "You have ran out of ammo", m_wolfList.Count, m_startTime);
            LevelManager.Instance.LoadNewLevel("EndScreen");
            return;
        }
    }

    public void NotifyAnimalsOfShot(Vector3 hitLoc)
    {
        foreach (var wolf in m_wolfList)
        {
            wolf.ReactToGunFire(hitLoc);
        }
        foreach (var pack in m_packOfSheep)
        {
            foreach (var sheep in pack.m_sheepList)
            {
                sheep.ReactToGunFire(hitLoc);
            }
        }
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
