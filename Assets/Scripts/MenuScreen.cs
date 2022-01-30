using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuScreen : MonoBehaviour
{
    public GameObject m_indicator;
    public GameObject m_menu;
    public GameObject m_select;


    public Toggle m_infiniteAmmoLegacy;

    // Start is called before the first frame update
    public void Play()
    {
        if(PlayerPrefs.GetInt("GameJamMode", 1) == 1)
        {
            LevelManager.Instance.LoadNewLevel("Main2");
        }
        else
        {
            m_select.SetActive(true);
            m_menu.SetActive(false);
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void Awake()
    {
        m_indicator.SetActive(PlayerPrefs.GetInt("GameJamMode", 1) == 0);
    }

    public void Update()
    {

        if (PlayerPrefs.GetInt("GameJamMode", 1) == 1)
        {
            m_indicator.SetActive(false);
            m_infiniteAmmoLegacy.gameObject.SetActive(true);
            GameManager.m_ammoMax = (m_infiniteAmmoLegacy.isOn ? -1 : 8);
        }
        else
        {
            m_indicator.SetActive(true);
            m_infiniteAmmoLegacy.gameObject.SetActive(false);
        }
    }
}
