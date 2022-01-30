using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButton : MonoBehaviour
{
    private bool isOpen = false;

    public void Open()
    {
        isOpen = !isOpen;
    }

    public void Close()
    {
        isOpen = false;
    }

    private void Update()
    {
        GetComponent<Animator>().SetBool("IsOpen", isOpen);
    }
}
