using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndApplication : MonoBehaviour
{
    /// <summary>
    /// Closes the App, only works in build version.
    /// </summary>
    public void QuitApp()
    {
        Application.Quit();
    }
}
