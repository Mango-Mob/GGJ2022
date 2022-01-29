using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScreen : MonoBehaviour
{
    // Start is called before the first frame update
    public void Play()
    {
        LevelManager.Instance.LoadNewLevel("Main");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
