using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Disables Animator and shows BookHandle at the end of the animation
/// </summary>

public class DisableAnimator : MonoBehaviour
{
    private Animator animator;
    public GameObject handleVisual;
    public GameObject handleLabel;
    public GameObject bookArmature;
    public GameObject effect;
    private BookRotation bookRotation;

    
    void Start()
    {
        animator = GetComponent<Animator>();
        handleVisual.SetActive(false);
        handleLabel.SetActive(false);
        effect.SetActive(true);

        bookRotation = bookArmature.GetComponent<BookRotation>();
        bookRotation.enabled = false;
    }

    // This method will be called by the animation event
    public void EnableInteractivity()
    {
        animator.enabled = false;
        handleVisual.SetActive(true);
        handleLabel.SetActive(true);
        effect.SetActive(false);
        bookRotation.enabled = true;
    }
}
