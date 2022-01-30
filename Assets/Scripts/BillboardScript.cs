using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BillboardScript : MonoBehaviour
{
    void Update()
    {
        transform.forward = GameManager.Instance.m_playerCamera.transform.forward;
    }
}
