using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoryDataScriptableObject", menuName = "ScriptableObject/StoryData", order = 1)]
public class StoryDataScriptableObject : ScriptableObject
{
    public List<StoryData> scenes;
 
    // Used to assign a special animation to a story scene
    public enum RotkäppchenAnimation {
        Walk,
        Teleport,
        BowDown,
    }
    public enum WolfAnimation {
        Walk,
        Teleport,
        Knock,
    }
    // Set Story data, can be done in inspector under /StoryData
    [System.Serializable]
    public class StoryData
    {
        public string text;
        public string textTranslation;
        [Tooltip("If true the player has to perform an action this scene instead of reading text.")]
        public bool interactiveScene;
        public AudioClip audio;
        public Vector3 rotkäppchenMoveTarget;
        public Vector3 wolfMoveTarget;
        public Vector3 newCameraPosition;
        public RotkäppchenAnimation rotkäppchenAnimation;
        public WolfAnimation wolfAnimation;
    }

}
