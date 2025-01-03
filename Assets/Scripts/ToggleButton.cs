using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Samples.StarterAssets;

public class SettingsButton : MonoBehaviour
{
    [SerializeField] private GameObject toogle;
    [Tooltip("The image directly under the button / the invisible image that needs to be pressed for button action")]
    [SerializeField] private GameObject image; 

    // Works with mouse but bugged in hand tracking right now
    public void SwitchState() 
    { 
        bool toogled = toogle.GetComponent<Toggle>().isOn;
        var pokeAffordance = toogle.GetComponent<XRPokeFollowAffordance>();
        float distance;

        if(toogled)
        {
            // lock button to pressed position
            distance = (float)-0.1;
        }
        else
        {
            distance = (float)-0.5;
        }
        
        pokeAffordance.maxDistance = distance;
        image.transform.localPosition = new Vector3( image.transform.localPosition.x, image.transform.localPosition.y, distance );
    }
}
