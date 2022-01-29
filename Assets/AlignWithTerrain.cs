using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlignWithTerrain : MonoBehaviour
{
    private void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, Vector3.down * 10f, out hit, 1 << LayerMask.NameToLayer("Ground"));

        transform.up = hit.normal;
    }
}
