using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Controller : MonoBehaviour
{
    private Player_Camera playerCamera;

    // Start is called before the first frame update
    void Start()
    {
        playerCamera = GetComponentInChildren<Player_Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        // Aim Camera
        Vector2 mouseMove = (InputManager.Instance as InputManager).GetMouseDelta();
        playerCamera.MoveCamera(mouseMove);

        // Shoot Gun


    }
}
