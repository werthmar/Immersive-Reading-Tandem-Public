//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

/// <summary>
/// Controlls movment and animations for each scene.
/// Made using Chatgpt and manually edited.
/// </summary>

public class NPCController : MonoBehaviour
{
    private Animator animator;            // Reference to the Animator
    private Vector3 targetPosition;     // Target position to move to
    public float moveSpeed = 10.0f;       // Speed of the character
    private bool isWalking = false;       // To check if the character is moving

    // Delegate for the event when walking is complete
    public delegate void AnimationComplete();
    public event AnimationComplete OnAnimationComplete; 

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (isWalking)
        {
            MoveToTarget();
        }
    }

    public void Walk(Vector3 target)
    {
        targetPosition = target;
        // Start walking
        isWalking = true; 
        animator.SetBool("isWalking", true);  // Trigger walk animation
    }

    public void PlayAnimation (AnimationClip clip)
    {
        animator.Play(clip.name);
        OnAnimationComplete?.Invoke();
    }

    private void MoveToTarget()
    {
        // Calculate the direction to the target
        Vector3 direction = (targetPosition - transform.position).normalized;

        // If the direction is not zero, update the rotation
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, moveSpeed * Time.deltaTime);
        }

        // Move toward the target position
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // Check if the character has reached the target position
        if(Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Stop walking
            isWalking = false;
            animator.SetBool("isWalking", false); // Switch back to idle animation
        
            // Notify StoryDataManager that walk is over
            OnAnimationComplete?.Invoke();
        }
    }
}
