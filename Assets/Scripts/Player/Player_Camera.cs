using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("General")]
    [SerializeField] [Range(1.0f, 120.0f)] private float m_lookFOV = 90.0f;
    [SerializeField] [Range(1.0f, 120.0f)] private float m_aimFOV = 45.0f;
    [SerializeField] private LayerMask m_gunTargetLayerMask;

    [Header("Player Settings")]
    [SerializeField] private Vector2 m_lookSensitivity = new Vector2(5.0f, 5.0f);
    [SerializeField] private Vector2 m_aimSensitivity = new Vector2(5.0f, 5.0f);

    [Header("Objects")]
    [SerializeField] private GameObject m_crosshair;

    private Camera m_camera;
    private float m_xRotation = 0.0f;
    public bool m_isScoped { get; private set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ShootGun()
    {
        RaycastHit[] hits = Physics.RaycastAll(transform.position, m_camera.transform.forward, 1000.0f, m_gunTargetLayerMask);
        
        Collider hitCollider = null;
        Vector3 hitPosition = Vector3.zero;
        if (hits.Length == 1)
        {
            hitCollider = hits[0].collider;
        }
        else
        {
            float closestDistance = Mathf.Infinity;
            foreach (var hit in hits)
            {
                float distance = Vector3.Distance(transform.position, hit.point);
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

        m_xRotation -= verticalMove;
        m_xRotation = Mathf.Clamp(m_xRotation, -90.0f, 90.0f);

        m_camera.transform.localRotation = Quaternion.Euler(m_xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up, horizontalMove);
    }
}
