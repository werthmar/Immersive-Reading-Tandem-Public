using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Hides / Shows the mand menu when palm is facing towards the head
/// !!! This Script was made using the following youtube tutorial: https://www.youtube.com/watch?v=hVuaYdWPGl8 !!!
/// </summary>
public class HandMenu : MonoBehaviour
{
    [SerializeField] private GameObject head;
    [SerializeField] private GameObject handPalm;
    [SerializeField] private GameObject menuCanvcas;

    private Vector3 dirFromAtoB = Vector3.zero;
    private float dotProd = 0f;

    // Start is called before the first frame update
    void Start()
    {
        menuCanvcas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        dirFromAtoB = (head.transform.position - handPalm.transform.position).normalized;
        dotProd = Vector3.Dot(dirFromAtoB, (-1) * handPalm.transform.up);

        if (dotProd > 0.9)
        {
            // Hand palm is facing camera
            if (!menuCanvcas.activeSelf) {
                menuCanvcas.SetActive(true);
            }
        }
        else if (menuCanvcas.activeSelf)
        {
            menuCanvcas.SetActive(false);
        }

    }
}
