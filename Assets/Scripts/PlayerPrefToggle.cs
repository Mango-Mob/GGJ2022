using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefToggle : MonoBehaviour
{
    public string prefId;
    public bool defaultVal;
    public Toggle m_toggle;

    // Start is called before the first frame update
    void Start()
    {
        m_toggle.isOn = PlayerPrefs.GetInt(prefId, (defaultVal) ? 1 : 0 ) == 1;
    }

    public void OnValueChange()
    {
        PlayerPrefs.SetInt(prefId, (m_toggle.isOn) ? 1 : 0);
    }
}
