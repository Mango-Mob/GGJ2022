using AudioSystem.Agents;
using AudioSystem.Managers;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPrefSlider : MonoBehaviour
{
    public string prefId;
    public Slider m_slider;
    public Text m_value;

    // Start is called before the first frame update
    void Start()
    {
        m_slider.minValue = 0.0f;
        m_slider.maxValue = 100.0f;
        m_slider.value = PlayerPrefs.GetFloat(prefId, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        m_value.text = Mathf.FloorToInt(m_slider.value).ToString();

        PlayerPrefs.SetFloat(prefId, m_slider.value);
    }

}
