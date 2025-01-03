using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class ObjectManager : MonoBehaviour
{
    [System.Serializable]
    public class GrabbableObject
    {
        public GameObject gameObject;
        public bool isCorrectObject;
    }

    public GrabbableObject[] grabbableObjects;
    public TextGenerator textGenerator;

    private void OnEnable()
    {
        foreach (var obj in grabbableObjects)
        {
            var interactable = obj.gameObject.GetComponent<XRGrabInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.AddListener((args) => OnObjectGrabbed(obj, args));
            }
            else
            {
                Debug.LogError("XRGrabInteractable is missing on " + obj.gameObject.name);
            }
        }
    }

    private void OnDisable()
    {
        foreach (var obj in grabbableObjects)
        {
            var interactable = obj.gameObject.GetComponent<XRGrabInteractable>();
            if (interactable != null)
            {
                interactable.selectEntered.RemoveListener((args) => OnObjectGrabbed(obj, args));
            }
        }
    }

    private void OnObjectGrabbed(GrabbableObject obj, SelectEnterEventArgs args)
    {
        if (obj.isCorrectObject)
        {
            Debug.Log("Correct object picked: " + obj.gameObject.name);
            textGenerator.DisplayInteractiveTaskResult(true, "Korrekt!");
        }
        else
        {
            Debug.Log("Incorrect object picked: " + obj.gameObject.name);
            textGenerator.DisplayInteractiveTaskResult(false, "Falsch, es sind <color=red>ROTE</color> ROSEN.");
        }
    }
}
