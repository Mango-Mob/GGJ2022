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
    
    // Dog things
    public float m_dogCD = 10.0f;
    public float m_dogCDTimer = 0.0f;
    private bool m_usedDogFlag = false;

    [Header("Player Settings")]
    [SerializeField] private Vector2 m_lookSensitivity = new Vector2(5.0f, 5.0f);
    [SerializeField] private Vector2 m_aimSensitivity = new Vector2(5.0f, 5.0f);

    [Header("Objects")]
    [SerializeField] private Camera m_camera;
    [SerializeField] private Camera m_clippingCamera;
    [SerializeField] private GameObject m_crosshair;
    [SerializeField] private Transform m_gunBarrelEnd;

    [Header("Prefab")]
    [SerializeField] private GameObject m_bulletPrefabVFX;
    [SerializeField] private GameObject m_dogPingPrefabVFX;

    private float m_xRotation = 0.0f;
    public bool m_isScoped { get; private set; } = false;

    // Recoil
    [Header("Recoil")]
    [SerializeField] [Range(0.0f, 1.0f)] private float m_scopedRecoilMult = 0.3f;
    [SerializeField] private float m_verticalRecoil = 1.0f;
    [SerializeField] private float m_horizontalRecoil = 1.0f;
    [SerializeField] private float m_recoilSmoothTime = 0.3f;
    [SerializeField] private Vector2 m_targetRecoilDown = new Vector2(0.0f, -10.0f);
    private Vector2 m_recoilVelocity = Vector2.zero;
    private float m_recoilAcceleration = 0.0f;
    private Vector2 m_recoilDampVelocity = Vector2.zero;

    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponentInParent<Player_Controller>();
    }

    private void Update()
    {
        // Recoil process
        AdjustCamera(m_recoilVelocity.x * Time.deltaTime, m_recoilVelocity.y * Time.deltaTime);

        m_recoilVelocity = Vector2.SmoothDamp(m_recoilVelocity, (m_recoilVelocity.y > 0.0f ? m_targetRecoilDown : Vector2.zero), ref m_recoilDampVelocity, m_recoilSmoothTime);

        if (m_usedDogFlag && GameManager.Instance.m_dog == null)
        {
            m_dogCDTimer = m_dogCD;
            m_usedDogFlag = false;
        }

        if (m_dogCDTimer > 0.0f)
            m_dogCDTimer -= Time.deltaTime;
        else
            m_dogCDTimer = 0.0f;
    }


    public void ShootGun()
    {
        m_recoilVelocity.y += m_verticalRecoil * (m_isScoped ? m_scopedRecoilMult : 1.0f);
        m_recoilVelocity.x += Random.Range(-m_horizontalRecoil, m_horizontalRecoil) * (m_isScoped ? m_scopedRecoilMult : 1.0f);

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
        {
            GameObject missBulletVFX = Instantiate(m_bulletPrefabVFX, (m_isScoped ? transform.position : m_gunBarrelEnd.position), m_camera.transform.rotation);
            return;
        }

        GameObject bulletVFX = Instantiate(m_bulletPrefabVFX, (m_isScoped ? transform.position : m_gunBarrelEnd.position), Quaternion.identity);
        bulletVFX.transform.forward = (hitPosition - (m_isScoped ? transform.position : m_gunBarrelEnd.position)).normalized;
        bulletVFX.GetComponent<BulletVFX>().SetEndPoint(hitPosition);

        GameManager.Instance.NotifyAnimalsOfShot(hitPosition);

        Sheep target = hitCollider.GetComponentInParent<Sheep>();
        if (target)
        {
            Debug.Log("Target Hit");
            
            string name = target.Kill(true);
            if (name != string.Empty)
            {
                KillFeedManager.Instance.DisplayPlayerKill(name);
            }

            if (target.GetComponent<Wolf>())
                GameManager.Instance.m_ammoCount++;
        }

        m_bulletPrefabVFX.transform.forward = m_camera.transform.forward;
    }
    public void ToggleScope(bool _active)
    {
        m_isScoped = _active;

        m_clippingCamera.enabled = !_active;

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
            horizontalMove = _mouseMove.x * m_lookSensitivity.x * PlayerPrefs.GetFloat("AimSens", 10.0f) / 10.0f;
            verticalMove = _mouseMove.y * m_lookSensitivity.y * PlayerPrefs.GetFloat("AimSens", 10.0f) / 10.0f; 
        }
        else 
        {
            horizontalMove = _mouseMove.x * m_aimSensitivity.x * PlayerPrefs.GetFloat("ScopeSens", 10.0f) / 10.0f;
            verticalMove = _mouseMove.y * m_aimSensitivity.y * PlayerPrefs.GetFloat("ScopeSens", 10.0f) / 10.0f;
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
        RaycastHit[] hits = Physics.RaycastAll(m_camera.transform.position, m_camera.transform.forward, 1000.0f, m_commandTargetLayerMask);

        if (hits.Length != 0)
        {
            if (!Dog.CreateDogToLoc(transform, hits[0].point))
            {
                return false;
            }

            GameObject vfx = Instantiate(m_dogPingPrefabVFX, hits[0].point, Quaternion.identity);
            vfx.transform.up = hits[0].normal;

            m_usedDogFlag = true;

            return true;
        }

        return false;
    }
}
