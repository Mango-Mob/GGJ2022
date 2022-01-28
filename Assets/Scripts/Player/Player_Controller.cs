using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int m_maxAmmo = 8;
    [SerializeField] private int m_maxDogCommands = 3;

    private Player_Camera playerCamera;
    private int m_ammoCount;
    private int m_dogCommands;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Player_Camera>();
        Cursor.lockState = CursorLockMode.Locked;

        m_ammoCount = m_maxAmmo;
        m_maxDogCommands = m_dogCommands;
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
        if (m_maxAmmo > 0 && InputManager.Instance.GetMouseDown(MouseButton.LEFT))
        {
            Debug.Log("Pew Pew");
            m_maxAmmo--;
            playerCamera.ShootGun();
        }
    }
}
