namespace MyFirstARGame
{
    using UnityEngine;
    using UnityEngine.XR.Interaction.Toolkit.AR;

    /// <summary>
    /// Helper class for interactables that disables certain components on non-mobile platforms to prevent certain synchronization issues.
    /// </summary>
    public class NetworkedInteractable : MonoBehaviour
    {
        private void Start()
        {
            if (!Application.isMobilePlatform)
            {
                if (this.GetComponent<ARSelectionInteractable>() != null)
                {
                    this.GetComponent<ARSelectionInteractable>().enabled = false;
                }
                if (this.GetComponent<ARTranslationInteractable>() != null)
                {
                    this.GetComponent<ARTranslationInteractable>().enabled = false;
                }
                if (this.GetComponent<ARRotationInteractable>() != null)
                {
                    this.GetComponent<ARRotationInteractable>().enabled = false;
                }
                if (this.GetComponent<ARScaleInteractable>() != null)
                {
                    this.GetComponent<ARScaleInteractable>().enabled = false;
                }
            }
        }
    }
}
