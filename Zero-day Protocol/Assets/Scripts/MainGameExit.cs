using UnityEngine;
using UnityEngine.SceneManagement;

public class MainGameEscapeDoor : MonoBehaviour
{
    [Header("Scene Configuration")]
    public string hubBaseSceneName = "map"; // Name of your player base scene layout
    public string failFallbackSceneName = "MainMenu_Scene"; // Route here if escaping without the card

    [Header("Progression Tracking")]
    [Tooltip("Which stage item are we collecting by escaping this level? (1, 2, or 3)")]
    public int currentStageNumber = 1;
    public string keycardItemName = "Stage1_Keycard"; // The exact ID your inventory system checks for

    private bool playerInZone = false;
    private PlayerController localPlayer; // Store player to check inventory safely

    void Update()
    {
        // When player stands at the door and presses E to escape
        if (playerInZone && Input.GetKeyDown(KeyCode.E) && localPlayer != null)
        {
            ProcessExtraction();
        }
    }

    void ProcessExtraction()
    {
        // Use the project's InventoryManager (InventoryController type does not exist)
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        if (inv == null && localPlayer != null)
        {
            inv = localPlayer.GetComponent<InventoryManager>();
        }

        if (inv != null)
        {
            // CHECK CONDITION: Did they secure the green keycard from the desk?
            bool hasKey = false;
            for (int i = 0; i < inv.itemSlots.Length; i++)
            {
                if (inv.itemSlots[i] != null && inv.itemSlots[i].itemName == keycardItemName)
                {
                    hasKey = true;
                    break;
                }
            }

            if (hasKey)
            {
                Debug.Log($"[EXIT] Extracted successfully WITH the key item: {keycardItemName}!");

                // Award the permanent campaign trophy flag dynamically
                AwardTrophy();

                // Clear all inventory slots so they start the next deployment completely fresh
                ClearAllInventorySlots(inv);

                // Load back into your hub base room / map scene safely
                SceneManager.LoadScene(hubBaseSceneName);
            }
            else
            {
                Debug.LogWarning("[EXIT] Player escaped but forgot the key item! Objective Failed.");

                // Lose all gear/temporary items gathered during this run anyway
                ClearAllInventorySlots(inv);

                // Boot them back out to a fallback scene or main menu
                SceneManager.LoadScene(failFallbackSceneName);
            }
        }
        else
        {
            Debug.LogError("[EXIT] Could not find an InventoryManager instance in the scene or on the player!");
        }
    }

    void AwardTrophy()
    {
        if (GameProgression.Instance != null)
        {
            if (currentStageNumber == 1)
            {
                GameProgression.Instance.stage1ItemCollected = true;
                Debug.Log("Stage 1 complete! Item flagged as collected.");
            }
            else if (currentStageNumber == 2)
            {
                GameProgression.Instance.stage2ItemCollected = true;
                Debug.Log("Stage 2 complete! Item flagged as collected.");
            }
            else if (currentStageNumber == 3)
            {
                GameProgression.Instance.stage3ItemCollected = true;
                Debug.Log("Stage 3 complete! Item flagged as collected.");
            }
        }
        else
        {
            Debug.LogWarning("PROGRESSION WARNING: GameProgression manager missing in scene. Level cleared but data won't persist!");
        }
    }

    // Helper to clear inventory compatible with InventoryManager implementation
    void ClearAllInventorySlots(InventoryManager inv)
    {
        for (int i = 0; i < inv.itemSlots.Length; i++)
        {
            inv.itemSlots[i] = null;
        }
        inv.UpdateInventoryUI();
    }

    // --- Trigger Zones ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = true;
            localPlayer = other.GetComponent<PlayerController>(); // Cache player reference
            Debug.Log("At Escape Door. Press 'E' to attempt extraction!");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInZone = false;
            localPlayer = null; // Clear cache out safely
        }
    }
}