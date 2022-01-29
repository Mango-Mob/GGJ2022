using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    public Text m_display;

    // Update is called once per frame
    void Update()
    {
        m_display.text = (DateTime.Now - GameManager.Instance.m_startTime).ToString("c").Substring(0, 12);
    }
}
