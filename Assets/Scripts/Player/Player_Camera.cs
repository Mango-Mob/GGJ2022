using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    private Player_Controller playerController;

    [Header("General")]
    [SerializeField] [Range(1.0f, 120.0f)] private float m_lookFOV = 90.0f;
    [SerializeField] [Range(1.0f, 120.0f)] private float m_aimFOV = 45.0f;
    [SerializeField] private LayerMask m_gunTargetLayerMask;
    [SerializeField] private LayerMask m_commandTargetLayerMask;

    [Header("Player Settings")]
    [SerializeField] private Vector2 m_lookSensitivity = new Vector2(5.0f, 5.0f);
    [SerializeField] private Vector2 m_aimSensitivity = new Vector2(5.0f, 5.0f);

    [Header("Objects")]
    [SerializeField] private GameObject m_crosshair;

    [Header("Prefab")]
    [SerializeField] private GameObject m_dogPingPrefabVFX;

    private Camera m_camera;
    private float m_xRotation = 0.0f;
    public bool m_isScoped { get; private set; } = false;

    // Recoil
    [Header("Recoil")]
    [SerializeField] private float m_verticalRecoil = 1.0f;
    [SerializeField] private float m_horizontalRecoil = 1.0f;
    [SerializeField] private float m_recoilSmoothTime = 0.3f;
    private Vector2 m_recoilVelocity = Vector2.zero;
    private Vector2 m_recoilDampVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInChildren<Camera>();
        playerController = GetComponentInParent<Player_Controller>();
    }

    private void Update()
    {
        // Recoil process
        AdjustCamera(m_recoilVelocity.x, m_recoilVelocity.y);

        m_recoilVelocity = Vector2.SmoothDamp(m_recoilVelocity, Vector2.zero, ref m_recoilDampVelocity, m_recoilSmoothTime);
    }

    public void ShootGun()
    {
        m_recoilVelocity.y += m_verticalRecoil;
        m_recoilVelocity.x += Random.Range(-m_horizontalRecoil, m_horizontalRecoil);

        RaycastHit[] hits = Physics.RaycastAll(m_camera.transform.position, m_camera.transform.forward, 1000.0f, m_gunTargetLayerMask);
        
        Collider hitCollider = null;
        Vector3 hitPosition = Vector3.zero;
        if (hits.Length == 1)
        {
            hitCollider = hits[0].collider;
            hitPosition = hits[0].point;
        }
        else
        {
            float closestDistance = Mathf.Infinity;
            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(m_camera.transform.position, hit.point);
                if (distance < closestDistance)
                {
                    hitCollider = hit.collider;
                    hitPosition = hit.point;
                    closestDistance = distance;
                }
            }
        }

        if (hitCollider == null)
            return;

        GameManager.Instance.NotifyAnimalsOfShot(hitPosition);

        Sheep target = hitCollider.GetComponentInParent<Sheep>();
        if (target)
        {
            Debug.Log("Target Hit");
            target.Kill(true);
        }
    }
    public void ToggleScope(bool _active)
    {
        m_isScoped = _active;

        if (m_crosshair != null)
            m_crosshair.SetActive(m_isScoped);

        m_camera.fieldOfView = !m_isScoped ? m_lookFOV : m_aimFOV;
    }
    public void MoveCamera(Vector2 _mouseMove)
    {
        float horizontalMove;
        float verticalMove;
        if (!m_isScoped)
        {
            horizontalMove = _mouseMove.x * m_lookSensitivity.x;
            verticalMove = _mouseMove.y * m_lookSensitivity.x;
        }
        else 
        {
            horizontalMove = _mouseMove.x * m_aimSensitivity.x;
            verticalMove = _mouseMove.y * m_aimSensitivity.x;
        }

        AdjustCamera(horizontalMove, verticalMove);
    }
    private void AdjustCamera(float _horizontal, float _vertical)
    {
        m_xRotation -= _vertical;
        m_xRotation = Mathf.Clamp(m_xRotation, -90.0f, 90.0f);

        m_camera.transform.localRotation = Quaternion.Euler(m_xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up, _horizontal);
    }
    public bool CommandDog()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, m_camera.transform.forward, 1000.0f, m_commandTargetLayerMask);

        if (hits.Length != 0)
        {
            Instantiate(m_dogPingPrefabVFX, hits[0].point, Quaternion.identity);

            Vector3 dogSpawn = transform.position;
            dogSpawn.y = 0.0f;
            Dog.CreateDogToLoc(dogSpawn, hits[0].point);
            return true;
        }

        return false;
    }
}
