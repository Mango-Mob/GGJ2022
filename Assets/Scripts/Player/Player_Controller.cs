using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player_Controller : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int m_maxAmmo = 8;
    [SerializeField] private int m_maxDogCommands = 3;

    private Player_Camera playerCamera;
    private int m_ammoCount;
    private int m_dogCommands;

    [Header("UI")]
    [SerializeField] private UI_BulletCount m_bulletCount;
    [SerializeField] private TextMeshProUGUI m_sheepCount;
    [SerializeField] private TextMeshProUGUI m_wolfCount;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Player_Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_ammoCount = m_maxAmmo;
        m_maxDogCommands = m_dogCommands;

        if (m_bulletCount != null)
        {
            m_bulletCount.SetupMaxAmmo(m_maxAmmo);
        }

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
        if (m_ammoCount > 0 && InputManager.Instance.GetMouseDown(MouseButton.LEFT))
        {
            Debug.Log("Pew Pew");
            m_ammoCount--;
            playerCamera.ShootGun();
        }

        if (GameManager.Instance.m_dog == null && InputManager.Instance.GetMouseDown(MouseButton.MIDDLE))
        {
            playerCamera.CommandDog();
        }


        // UI Update
        if (m_bulletCount != null)
        {
            m_bulletCount.SetAmmoCount(m_ammoCount);
        }
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
