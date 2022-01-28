using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Camera : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private Vector2 m_lookSensitivity = new Vector2(5.0f, 5.0f);
    [SerializeField] private Vector2 m_aimSensitivity = new Vector2(5.0f, 5.0f);

    private Camera m_camera;
    private float m_xRotation = 0.0f;
    private bool m_isScoped = false;

    // Start is called before the first frame update
    void Start()
    {
        m_camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveCamera(Vector2 _mouseMove)
    {
        float horizontalMove = 0.0f;
        float verticalMove = 0.0f;
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

        m_camera.transform.localRotation = Quaternion.Euler(m_xRotation, 0.0f, 0.0f);
        m_camera.transform.Rotate(Vector3.up, horizontalMove);
    }
}
