using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AmmoCount : MonoBehaviour
{
    public Image[] m_bulletSprites;
    private int m_lastAmmoCount = 8;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        SetAmmoCount(GameManager.Instance.m_ammoCount);
    }
    public void SetAmmoCount(int _ammoCount)
    {
        if (m_lastAmmoCount != _ammoCount)
        {
            m_lastAmmoCount = _ammoCount;

            for (int i = 0; i < m_bulletSprites.Length; i++)
            {
                if (i >= m_lastAmmoCount)
                {
                    m_bulletSprites[i].enabled = false;
                }
                else
                {
                    m_bulletSprites[i].enabled = true;
                }
            }
        }
    }
}
