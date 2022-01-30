using AudioSystem.Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject m_menu;
    public static bool isPaused = false;
    // Update is called once per frame
    void Update()
    {
        if(InputManager.Instance.IsKeyDown(KeyType.ESC) && PlayerPrefs.GetInt("GameJamMode", 1) != 1)
        {
            isPaused = !isPaused;
        }

        m_menu.SetActive(isPaused);

        Cursor.lockState = (isPaused) ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = isPaused;
    }

    public void ReturnToMenu()
    {
        isPaused = false;
        LevelManager.Instance.LoadNewLevel("MainMenuScreen");
    }
}
