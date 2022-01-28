﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AudioSystem.Agents;

public class Player_Controller : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int m_maxDogCommands = 3;

    private Player_Camera playerCamera;
    public MultiAudioAgent audioAgent { get; private set; }
    private int m_dogCommands;

    [Header("UI")]
    [SerializeField] private UI_AmmoCount m_ammoCount;
    [SerializeField] private UI_BulletCount m_bulletCount;
    [SerializeField] private TextMeshProUGUI m_sheepCount;
    [SerializeField] private TextMeshProUGUI m_wolfCount;

    // Start is called before the first frame update
    void Start()
    {
        audioAgent = GetComponent<MultiAudioAgent>();
        playerCamera = GetComponentInChildren<Player_Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_maxDogCommands = m_dogCommands;

        //m_ammoCount = m_maxAmmo;
        //if (m_bulletCount != null)
        //{
        //    m_bulletCount.SetupMaxAmmo(m_maxAmmo);
        //}

    }

    // Update is called once per frame
    void Update()
    {
        // Aim Camera
        Vector2 mouseMove = InputManager.Instance.GetMouseDelta();
        playerCamera.MoveCamera(mouseMove * Time.deltaTime);

        // Scope
        if (InputManager.Instance.GetMouseDown(MouseButton.RIGHT))
        {
            playerCamera.ToggleScope(!playerCamera.m_isScoped);
        }

        // Shoot Gun
        if (GameManager.Instance.m_ammoCount > 0 && InputManager.Instance.GetMouseDown(MouseButton.LEFT))
        {
            Debug.Log("Pew Pew");
            GameManager.Instance.m_ammoCount--;
            playerCamera.ShootGun();

            audioAgent.Play("Gunshot");
        }

        if (GameManager.Instance.m_dog == null && InputManager.Instance.GetMouseDown(MouseButton.MIDDLE))
        {
            if (playerCamera.CommandDog())
            {
                audioAgent.Play("DogCall");
            }
            else
            {

            }
        }

        // UI Update
        if (m_sheepCount != null)
        {
            m_sheepCount.text = $"{GameManager.Instance.GetSheepCount()} Sheep";
        }
        if (m_wolfCount != null)
        {
            m_wolfCount.text = $"{GameManager.Instance.m_wolfList.Count} Wolves";
        }
    }
}
