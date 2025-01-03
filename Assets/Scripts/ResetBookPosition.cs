using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetBookPosition : MonoBehaviour
{
    [Tooltip("Use index finger")]
    [SerializeField] private GameObject resetPosition;
    [SerializeField] private GameObject magicBook;
    

    public void ResetBook() {
        magicBook.transform.position = resetPosition.transform.position;
    }
}
