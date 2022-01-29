using AudioSystem.Agents;
using AudioSystem.Managers;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioManager.VolumeChannel m_channel;
    public Slider m_slider;
    public Text m_value;

    private SoloAudioAgent m_agent;
    private bool IsAwake = false;
    // Start is called before the first frame update
    void Start()
    {
        m_agent = GetComponent<SoloAudioAgent>();
        m_agent.channel = m_channel;

        m_slider.minValue = 0.0f;
        m_slider.maxValue = 100.0f;
        m_slider.value = AudioManager.Instance.volumes[(int)m_channel] * 100f;
        IsAwake = true;
    }

    // Update is called once per frame
    void Update()
    {
        m_value.text = Mathf.FloorToInt(m_slider.value).ToString();

        AudioManager.Instance.volumes[(int)m_channel] = m_slider.value / 100f;
    }

    public void OnValueChange()
    {
        if (!IsAwake)
            return;

        if(m_agent.mainClip != null)
            m_agent.PlayOnce();
    }
}
