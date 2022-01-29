﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Movement : MonoBehaviour
{
    [SerializeField] private float m_moveSpeed = 10.0f;
    [SerializeField] private float m_gravitySpeed = 9.81f;
    [SerializeField] [Range(0.0f, 1.0f)] private float m_scopeMoveMult = 0.3f;


    private CharacterController characterController;
    private Player_Controller playerController;
    float m_yVelocity = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerController = GetComponentInParent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!characterController.isGrounded)
            m_yVelocity -= m_gravitySpeed * Time.deltaTime;
        else
            m_yVelocity = -1.0f;

        characterController.Move(Vector3.up * m_yVelocity);
    }
    public void Move(float _x, float _z)
    {
        Vector3 move = transform.right * _x + transform.forward * _z;

        characterController.Move(move * m_moveSpeed * Time.deltaTime * (playerController.playerCamera.m_isScoped ? m_scopeMoveMult : 1.0f));
    }
}
