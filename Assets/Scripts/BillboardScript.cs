using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardScript : MonoBehaviour
{
    void Update()
    {
        transform.forward = GameManager.Instance.m_playerCamera.transform.forward;
    }
}
