using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSpinner : MonoBehaviour
{
    public float rotateSpeed = 100f; // Speed of the rotation

    void Update()
    {
        // Rotate the image around its Z-axis
        transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
    }
}
