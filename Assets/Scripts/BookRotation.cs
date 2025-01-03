using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script was generated using ChatGPT.
/// </summary>

public class BookRotation : MonoBehaviour
{
    private Camera mainCamera;
    public Vector3 rotationOffset = Vector3.zero;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        Vector3 direction = mainCamera.transform.position - transform.position;
        //direction.y = 0; // Ignore y-axis rotation if not needed

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion offsetRotation = Quaternion.Euler(rotationOffset);
            transform.rotation = targetRotation * offsetRotation;
        }
    }
}
