using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverScreen : MonoBehaviour
{
    public enum ScreenState { Victory, Defeat };
    
    public Text m_title;
    public Text m_subTitle;
    public Text m_remainingStat;
    public Text m_timeStat;

    protected static int remainEntities;
    protected static DateTime startTime;
    protected static DateTime endTime;
    protected static ScreenState m_state;
    protected static string m_stateReason;

    // Start is called before the first frame update
    void Start()
    {
        switch (m_state)
        {
            case ScreenState.Victory:
                m_title.text = "Victory";
                m_subTitle.enabled = false;
                m_remainingStat.text = "Remaining Sheep: ";
                m_timeStat.text = "Time: ";
                break;
            case ScreenState.Defeat:
                m_title.text = "Defeat";
                m_subTitle.text = m_stateReason;
                m_remainingStat.text = "Remaining Wolf: ";
                m_timeStat.text = "Time: ";
                break;
            default:
                break;
        }
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
