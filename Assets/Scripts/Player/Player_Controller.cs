using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AudioSystem.Agents;

public class Player_Controller : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int m_maxDogCommands = 3;

    public Player_Camera playerCamera { get; private set; }
    public Player_Movement playerMovement { get; private set; }
    public MultiAudioAgent audioAgent { get; private set; }
    private int m_dogCommands;

    private bool m_reloading = false;
    [SerializeField] private float m_reloadDelay = 0.5f;

    [Header("UI")]
    [SerializeField] private UI_DogStatus m_dogStatus;
    [SerializeField] private UI_AmmoCount m_ammoCount;
    [SerializeField] private UI_BulletCount m_bulletCount;
    [SerializeField] private TextMeshProUGUI m_sheepCount;
    [SerializeField] private TextMeshProUGUI m_wolfCount;
    // Start is called before the first frame update
    void Start()
    {
        audioAgent = GetComponent<MultiAudioAgent>();
        playerCamera = GetComponentInChildren<Player_Camera>();
        playerMovement = GetComponentInChildren<Player_Movement>();

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
#if UNITY_EDITOR
        if (InputManager.Instance.IsKeyDown(KeyType.R))
        {
            GameManager.Instance.m_ammoCount++;
        }
#endif

        // Aim Camera
        Vector2 mouseMove = InputManager.Instance.GetMouseDelta();
        playerCamera.MoveCamera(mouseMove * Time.deltaTime);

        // Scope
        if (!m_reloading && InputManager.Instance.GetMouseDown(MouseButton.RIGHT))
        {
            playerCamera.ToggleScope(!playerCamera.m_isScoped);
        }

        // Shoot Gun
        if (!m_reloading && GameManager.Instance.m_ammoCount > 0 && InputManager.Instance.GetMouseDown(MouseButton.LEFT))
        {
            GameManager.Instance.m_ammoCount--;
            playerCamera.ShootGun();


            audioAgent.Play("Gunshot");
            StartCoroutine(ReloadRifle());
        }

        if (InputManager.Instance.GetMouseDown(MouseButton.MIDDLE))
        {
            if (playerCamera.m_dogCDTimer <= 0.0f && GameManager.Instance.m_dog == null)
            {
                if (playerCamera.CommandDog())
                {
                    audioAgent.Play("DogCall");
                }
                else
                {

                }
            }
        }

        Vector2 move = GetMovementAxis();
        playerMovement.Move(move.x, move.y);

        // UI Update
        if (m_dogStatus != null)
        {
            m_dogStatus.UpdateDogCooldown(playerCamera.m_dogCDTimer / playerCamera.m_dogCD);
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
    IEnumerator ReloadRifle()
    {
        m_reloading = true;
        yield return new WaitForSecondsRealtime(m_reloadDelay);

        playerCamera.ToggleScope(false);
        audioAgent.Play("RifleLoad");

        yield return new WaitForSecondsRealtime(0.3f);
        m_reloading = false;
    }

    static Vector2 GetMovementAxis()
    {
        Vector2 move = Vector2.zero;

        move.x = (InputManager.Instance.IsKeyPressed(KeyType.D) ? 1.0f : 0.0f) - (InputManager.Instance.IsKeyPressed(KeyType.A) ? 1.0f : 0.0f);
        move.y = (InputManager.Instance.IsKeyPressed(KeyType.W) ? 1.0f : 0.0f) - (InputManager.Instance.IsKeyPressed(KeyType.S) ? 1.0f : 0.0f);

        return move;
    }

}
