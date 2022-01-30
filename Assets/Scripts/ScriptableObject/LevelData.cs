using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelData : ScriptableObject
{
    public string m_sceneName;
    public string m_levelName;

    public int m_maxWolfCount;
    public int m_maxSheepCount;
}
