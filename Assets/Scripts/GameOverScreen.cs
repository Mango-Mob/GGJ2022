using AudioSystem.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ScreenState { Victory, Defeat };

public class GameOverScreen : MonoBehaviour
{
    public Text m_title;
    public Text m_subTitle;
    public Text m_remainingStat;
    public Text m_timeStat;
    public Text m_bulletCountText;

    protected static int remainEntities;
    protected static DateTime startTime;
    protected static DateTime endTime;
    protected static ScreenState m_state;
    protected static string m_stateReason;

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.m_globalVolume = 1.0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        switch (m_state)
        {
            case ScreenState.Victory:
                m_title.text = "Victory";
                m_subTitle.text = m_stateReason;
                m_remainingStat.text = $"Remaining Sheep: {remainEntities}";
                m_timeStat.text = $"Time: {(endTime - startTime).ToString("c").Substring(0, 12)}";
                break;
            case ScreenState.Defeat:
                m_title.text = "Defeat";
                m_subTitle.text = m_stateReason;
                m_remainingStat.text = $"Remaining Wolves:  {remainEntities}";
                m_timeStat.text = $"Time: {(endTime - startTime).ToString("c").Substring(0, 12)}";
                break;
            default:
                break;
        }
        if (PlayerPrefs.GetInt("GameJamMode", 1) == 0)
        {
            if(GameManager.m_ammoMax > 0)
                m_bulletCountText.text = $"Bullets remaining: {GameManager.Instance.m_ammoCount} (out of {GameManager.m_ammoMax})";
            else
                m_bulletCountText.text = $"Bullets remaining: \u221E (out of \u221E)";
        }
        else
        {
            m_bulletCountText.gameObject.SetActive(false);
        }
            
    }
    public void Play()
    {
        LevelManager.Instance.LoadNewLevel(GameManager.m_lastLevel.m_sceneName);
    }

    public void Return()
    {
        LevelManager.Instance.LoadNewLevel("MainMenuScreen");
    }

    public static void SetScene(ScreenState _state, string reason, int _remaining, DateTime _startTime)
    {
        m_state = _state;
        m_stateReason = reason;
        remainEntities = _remaining;
        startTime = _startTime;
        endTime = DateTime.Now;
    }
}
