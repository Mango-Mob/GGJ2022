using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using AudioSystem.Agents;

public class Player_Controller : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private int m_maxDogCommands = 3;

    public Animator m_animator { get; private set; }
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
        m_animator = GetComponentInChildren<Animator>();
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
        if (PauseMenu.isPaused)
            return;

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
        if (m_animator.GetBool("CanShoot"))
        {
            if (PlayerPrefs.GetInt("ToggleAim") == 1)
            {
                if (!m_reloading && InputManager.Instance.GetMouseDown(MouseButton.RIGHT))
                {
                    if (playerCamera.m_isScoped)
                    {
                        //m_animator.SetBool("IsScoping", false);
                        m_animator.SetTrigger("StartScope");
                    }
                    else
                    {
                        //m_animator.SetBool("IsScoping", true);
                        m_animator.SetTrigger("StopScope");
                    }

                    //playerCamera.ToggleScope(!playerCamera.m_isScoped);
                }
            }
            else
            {
                if (InputManager.Instance.GetMousePress(MouseButton.RIGHT))
                {
                    if (!playerCamera.m_isScoped && !m_reloading)
                        m_animator.SetTrigger("StartScope");
                    //playerCamera.ToggleScope(true);
                }
                else
                {
                    if (playerCamera.m_isScoped)
                        m_animator.SetTrigger("StopScope");
                    //playerCamera.ToggleScope(false);
                }
            }

            // Shoot Gun
            if (!m_reloading && GameManager.Instance.m_ammoCount > 0 && InputManager.Instance.GetMouseDown(MouseButton.LEFT))
            {
                if (GameManager.m_ammoMax != -1)
                    GameManager.Instance.m_ammoCount--;
                playerCamera.ShootGun();

                audioAgent.Play("Gunshot");
                m_animator.SetTrigger("Shoot");
                StartCoroutine(ReloadRifle());
            }
        }
        else
        {
            //if (playerCamera.m_isScoped)
            //    m_animator.SetTrigger("StopScope");
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

    public void SetScope(bool _true)
    {
        playerCamera.ToggleScope(_true);
        if (!_true && m_reloading)
        {
            m_animator.SetTrigger("Reload");
        }
    }
    IEnumerator ReloadRifle()
    {
        m_reloading = true;
        yield return new WaitForSecondsRealtime(m_reloadDelay);

        if (playerCamera.m_isScoped)
            m_animator.SetTrigger("StopScope");
        else
            m_animator.SetTrigger("Reload");
    }
    public void StartReload()
    {
        audioAgent.Play("RifleLoad");
    }
    public void StopReload()
    {
        m_reloading = false;
    }
    static Vector2 GetMovementAxis()
    {
        Vector2 move = Vector2.zero;

        move.x = (InputManager.Instance.IsKeyPressed(KeyType.D) ? 1.0f : 0.0f) - (InputManager.Instance.IsKeyPressed(KeyType.A) ? 1.0f : 0.0f);
        move.y = (InputManager.Instance.IsKeyPressed(KeyType.W) ? 1.0f : 0.0f) - (InputManager.Instance.IsKeyPressed(KeyType.S) ? 1.0f : 0.0f);

        if (move.magnitude > 0.0f)
            return move.normalized;
        else
            return Vector2.zero;
    }

}
