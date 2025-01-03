using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SettingsToggle : MonoBehaviour
{
    [SerializeField] private GameObject tutorialCanvas;
    [SerializeField] private GameObject settingsCanvas;

    void Start()
    {
        tutorialCanvas.SetActive(true);
        settingsCanvas.SetActive(false);
    }

    public void SwitchCanvas()
    {
        tutorialCanvas.SetActive( !tutorialCanvas.activeSelf );
        settingsCanvas.SetActive( !settingsCanvas.activeSelf );
    }
}
