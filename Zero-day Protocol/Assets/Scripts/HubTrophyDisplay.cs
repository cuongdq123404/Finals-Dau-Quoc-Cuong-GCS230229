using UnityEngine;

public class HubTrophyDisplay : MonoBehaviour
{
    [Header("Trophy Stage Target")]
    [Tooltip("Which stage number does this trophy object represent?")]
    public int stageNumber = 1;

    [Header("Visual References")]
    [Tooltip("The actual child GameObject holding the sprite renderer for the trophy.")]
    public GameObject trophyVisualObject;

    void Start()
    {
        UpdateTrophyVisibility();
    }

    public void UpdateTrophyVisibility()
    {
        // 1. Safety check: make sure the visual container is assigned
        if (trophyVisualObject == null)
        {
            Debug.LogWarning($"[Trophy System] Visual object not assigned on {gameObject.name}!");
            return;
        }

        // 2. Check the global persistent progression tracker
        if (GameProgression.Instance != null)
        {
            bool isCollected = false;

            // Route to the correct boolean parameter depending on the stage configured in the inspector
            if (stageNumber == 1) isCollected = GameProgression.Instance.stage1ItemCollected;
            else if (stageNumber == 2) isCollected = GameProgression.Instance.stage2ItemCollected;
            else if (stageNumber == 3) isCollected = GameProgression.Instance.stage3ItemCollected;

            // 3. Turn the trophy artwork ON or OFF based on collection status
            if (isCollected)
            {
                trophyVisualObject.SetActive(true);
                Debug.Log($"[Trophy System] Stage {stageNumber} Trophy verified and DISPLAYED in base!");
            }
            else
            {
                trophyVisualObject.SetActive(false);
                Debug.Log($"[Trophy System] Stage {stageNumber} Trophy not earned yet. Hiding visual asset.");
            }
        }
        else
        {
            // If you test the Hub scene directly without coming from the Main Menu, the manager won't exist
            trophyVisualObject.SetActive(false);
            Debug.LogWarning("[Trophy System] GameProgression manager missing! Hiding trophy asset by default.");
        }
    }
}