using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace UnityEngine.XR.Hands
{
    /// <summary>
    /// Used to show EITHER the controllers or the hands based on what is being used
    /// append to the Left Hand Interaction Visual Object or the Right Hand interaction Visual
    /// add the corresponding controller to the script
    /// </summary>
    public class XRHandControllerToggle : MonoBehaviour
    {
    
        public GameObject controllerModel;

        private XRHandMeshController meshController;

        private bool handsTracked;

        void Start()
        {
            meshController = GetComponent<XRHandMeshController>();
            handsTracked = meshController.handIsTracked;
        }

        void Update()
        {
            handsTracked = meshController.handIsTracked;
            // Check and update visibility each frame
            UpdateHandControllerVisibility();
        }

        void UpdateHandControllerVisibility()
        {
            // If not hand tracking, default to controllers
            controllerModel.SetActive(!handsTracked);
        }
    }
}