using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Holds each text and example audio file of the story and allows access to them
/// Made using ChatGPT and edited manually
/// </summary>
public class StoryDataManager : MonoBehaviour
{
    // Custom animations, contains all available
    [Tooltip("Position in Array = Position in Dropdown")]
    public AnimationClip[] allRotkäppchenAnimations;
    public AnimationClip[] allWolfAnimations;

    // References to all controllable characters
    public NPCController rotkäppchen;
    public NPCController wolf;
    private bool rotkäppchenAnimationComplete;
    private bool wolfAnimationComplete;

    // Reference to the entire player GameObject
    public GameObject player;

    // Load, Get and Manage the current story snippet
    public StoryDataScriptableObject storyDataList; // Reference to your ScriptableObject
    public AudioSource audioSource;
    public int storyIndex = 0;
    public TextGenerator textGenerator;

    // For fade effect during story scene transition
    public GameObject fadeQuad; // Reference to the Quad GameObject for fading
    public float fadeDuration = 1.0f; // Duration for the fade effect
    private Renderer _renderer;
    private Material _material;
    private Color _color;

    // For fixing issue where coroutine gets called multiple times
    private bool isCoroutineRunning = false;

    public string GetStoryText()
    {
        return storyDataList.scenes[storyIndex].text;
    }

    public string GetStoryTextTranslation()
    {
        return storyDataList.scenes[storyIndex].textTranslation;
    }

    public void PlayStoryAudio()
    {
        AudioClip clip = storyDataList.scenes[storyIndex].audio;

        if (audioSource != null && clip != null)
        {
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    // Switch to next scene
    public void NextScene()
    {
        StoryDataScriptableObject.StoryData currentScene = storyDataList.scenes[storyIndex];

        textGenerator.Loading(true);
        isCoroutineRunning = false;

        rotkäppchenAnimationComplete = false;
        wolfAnimationComplete = false;

        // Subscribe to the OnWalkComplete event
        rotkäppchen.OnAnimationComplete += () => CompleteAnimation("rotkäppchen");

        // Check if we need to play a custom animation
        int rkAnimationIndex = (int) currentScene.rotkäppchenAnimation; // Get the index of the selected animation
        if (rkAnimationIndex == 0) // 0 = Walk
        {
            Vector3 rotkäppchenTargetPosition = currentScene.rotkäppchenMoveTarget;
            rotkäppchen.Walk(rotkäppchenTargetPosition);
        }
        else if (rkAnimationIndex == 1) // Teleport
        {
            rotkäppchen.transform.position = currentScene.rotkäppchenMoveTarget;
            rotkäppchen.PlayAnimation(allRotkäppchenAnimations[rkAnimationIndex+1]); // Bow down
        }
        else // Play custom animation
        {
            rotkäppchen.PlayAnimation(allRotkäppchenAnimations[rkAnimationIndex]);
        }

        int wolfAnimationIndex = (int) currentScene.wolfAnimation;
        // Move wolf but only if he needs to be moved in this scene
        Vector3 wolfTargetPosition = currentScene.wolfMoveTarget;
        if ( wolfTargetPosition != Vector3.zero && wolfAnimationIndex == 0)
        {
            wolf.Walk(wolfTargetPosition);
            wolf.OnAnimationComplete += () => CompleteAnimation("wolf");
        }
        else if (wolfAnimationIndex == 1) // Teleport
        {
            wolf.transform.position = wolfTargetPosition;
            CompleteAnimation("wolf");
        }
        else if (wolfAnimationIndex == 2) { // Knock animation
            wolf.PlayAnimation(allWolfAnimations[wolfAnimationIndex]);
            wolf.OnAnimationComplete += () => CompleteAnimation("wolf");
        }
        else {
            CompleteAnimation("wolf");
        }

    }

    private void CompleteAnimation(string character)
    {
        if (character == "wolf") {
            wolfAnimationComplete = true;
        }
        else {
            rotkäppchenAnimationComplete = true;
        }

        if (rotkäppchenAnimationComplete && wolfAnimationComplete)
        {
            wolf.transform.LookAt(rotkäppchen.transform);
            rotkäppchen.transform.LookAt(wolf.transform);
            MovePlayer();
        }

    }

    // Method to move the player
    private void MovePlayer()
    {
        Vector3 targetPosition = storyDataList.scenes[storyIndex].newCameraPosition;
        // Start the coroutine to smoothly move the player to the new position
        StartCoroutine( SmoothMovePlayer( targetPosition, PrepareNextScene ) );
        
    }

    private void PrepareNextScene()
    {
        // Load new story scene and rebuild text
        storyIndex++;

        if(storyIndex == storyDataList.scenes.Count - 1)
        {
            textGenerator.Initialize(false, true);
        }
        else
        {
            textGenerator.Initialize(storyDataList.scenes[storyIndex].interactiveScene);
        }

        textGenerator.Loading(false);
    }
    
    private IEnumerator SmoothMovePlayer(Vector3 targetPosition, Action onComplete)
    {
        // Fade to black
        _renderer = fadeQuad.GetComponent<Renderer>();
        _material = _renderer.material;
        _color = _material.color;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            _color.a = Mathf.Clamp01(elapsed / fadeDuration);
            _renderer.material.color = _color;
            yield return null;
        }

        // Move the player to new location
        player.transform.position = targetPosition;

        // Rotate the player to face Rotkäppchen
        Vector3 rotkäppchenPosition = rotkäppchen.transform.position;
        //player.transform.LookAt(rotkäppchenPosition);
        Vector3 direction = rotkäppchenPosition - transform.position;        
        // Ignore y component by setting it to zero
        direction.y = 0;

        if(direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            player.transform.rotation = targetRotation;
        }


        // Prepare next scene
        if(!isCoroutineRunning)
        {
            isCoroutineRunning = true;
            onComplete?.Invoke();
        }
        
        // Fade back in
        _color.a = 1; // Ensure material is fully opaque before starting
        _renderer.material.color = _color;

        elapsed = fadeDuration;

        while (elapsed > 0f)
        {
            elapsed -= Time.deltaTime;
            _color.a = Mathf.Clamp01(elapsed / fadeDuration);
            _renderer.material.color = _color;
            yield return null;
        }


    }

}
