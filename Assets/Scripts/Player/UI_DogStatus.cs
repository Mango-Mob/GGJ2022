using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_DogStatus : MonoBehaviour
{
    [Header("Dog State")]
    [SerializeField] private Image m_stateImage;
    [SerializeField] private Sprite m_idleSprite;
    [SerializeField] private Sprite m_moveSprite;
    [SerializeField] private Sprite m_barkSprite;
    [SerializeField] private Sprite m_restSprite;

    [Header("Cooldown")]
    [SerializeField] private Image m_cooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDogState();
    }
    void UpdateDogState()
    {
        if (GameManager.Instance.m_dog == null)
        {
            m_stateImage.sprite = m_restSprite;
            return;
        }

        switch (GameManager.Instance.m_dog.m_currentState)
        {
            case Dog.DogState.Scout:
                m_stateImage.sprite = m_moveSprite;
                break;
            case Dog.DogState.Wait:
                m_stateImage.sprite = m_idleSprite;
                break;
            case Dog.DogState.Follow:
                m_stateImage.sprite = m_barkSprite;
                break;
            case Dog.DogState.Return:
                m_stateImage.sprite = m_moveSprite;
                break;
            default:
                break;
        }
    }

    public void UpdateDogCooldown(float _percentage)
    {
        m_cooldown.fillAmount = _percentage;
    }
}
