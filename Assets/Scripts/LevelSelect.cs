using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelect : MonoBehaviour
{
    public LevelData m_mainLevel;
    public Text m_levelTitle;
    public Slider m_difficulty;
    public Slider m_wolfSlider;
    public Slider m_sheepSlider;

    public Text m_difficultyDisplay;
    public Text m_wolfDisplay;
    public Text m_sheepDisplay;

    private void Awake()
    {
        GameManager.m_lastLevel = m_mainLevel;
    }
    // Start is called before the first frame update
    void Start()
    {
        m_levelTitle.text = m_mainLevel.m_levelName;
        m_difficulty.minValue = 0;
        m_difficulty.maxValue = 5;
        m_difficulty.value = 2;
        m_difficulty.wholeNumbers = true;

        m_wolfSlider.minValue = 1;
        m_wolfSlider.maxValue = m_mainLevel.m_maxWolfCount;
        m_wolfSlider.wholeNumbers = true;
        m_wolfSlider.value = 4;
        
        m_sheepSlider.minValue = 1;
        m_sheepSlider.maxValue = m_mainLevel.m_maxSheepCount;
        m_sheepSlider.wholeNumbers = true;
        m_sheepSlider.value = 40;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.m_wolfSpawnCount == 1)
        {
            m_difficulty.maxValue = 3;
        } 
        else if (GameManager.m_wolfSpawnCount == 2)
        {
            m_difficulty.maxValue = 4;
        }
        else
        {
            m_difficulty.maxValue = 5;
        }

        int bullets = CalculateBulletDifficulty((int)m_difficulty.value);
        m_difficultyDisplay.text = (bullets > 0) ? $"{bullets.ToString()} bullets" : $" \u221E bullets";

        m_wolfDisplay.text = ((int)m_wolfSlider.value).ToString();
        m_sheepDisplay.text = ((int)m_sheepSlider.value).ToString();

        GameManager.m_wolfSpawnCount = (int)m_wolfSlider.value;
        GameManager.m_sheepSpawnCount = (int)m_sheepSlider.value;
        GameManager.m_ammoMax = bullets;
    }

    public void PlayLevel()
    {
        LevelManager.Instance.LoadNewLevel(m_mainLevel.m_sceneName);
    }

    private int CalculateBulletDifficulty(int index)
    {
        switch (index)
        {
            case 0:
                //Infinite
                return -1;
            case 1:
                //Four Per Wolf
                return 4 * GameManager.m_wolfSpawnCount;
            case 2:
                //Two per Wolf
                return 2 * GameManager.m_wolfSpawnCount;
            case 3:
                //One per Wolf
                return GameManager.m_wolfSpawnCount;
            case 4:
                //Half of wolf count
                return Mathf.Max(Mathf.FloorToInt(0.5f * GameManager.m_wolfSpawnCount), 1);
            case 5:
                //One
                return 1;
            default:
                return 0;
        }
    }
}
