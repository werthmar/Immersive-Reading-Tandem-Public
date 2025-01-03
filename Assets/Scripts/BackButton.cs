using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackButton : MonoBehaviour
{
    public Canvas canvas1;
    public Canvas canvas2;

    public void CanvasBack()
    {
        canvas1.gameObject.SetActive(!canvas1.isActiveAndEnabled);
        canvas2.gameObject.SetActive(!canvas1.isActiveAndEnabled);
    }
}
