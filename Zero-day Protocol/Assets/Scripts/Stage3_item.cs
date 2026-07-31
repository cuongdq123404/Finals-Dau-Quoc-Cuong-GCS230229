using UnityEngine;

public class Stage3Item : MonoBehaviour
{
    [Header("Item Config")]
    [Tooltip("Assign the ItemData scriptable object asset for the Stage 2 Keycard here.")]
    public ItemData Stage3_Item;

    private bool isPlayerInRange = false;
    private PlayerController localPlayer;

    void Update()
    {
        // When standing near the keycard and hitting 'E', pick it up
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && localPlayer != null)
        {
            PickUpStage3Keycard();
        }
    }

    void PickUpStage3Keycard()
    {
        if (Stage3_Item == null)
        {
            Debug.LogError("[Stage 3 Key] keycardItem data asset is not assigned in the Inspector!");
            return;
        }

        // Use the project's InventoryManager 
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();

        if (inv != null)
        {
            bool added = inv.AddItem(Stage3_Item);
            if (added)
            {
                Debug.Log($"[Stage 2 Key] {Stage3_Item.itemName} successfully added to inventory!");

                // --- STAGE PROGRESSION SYSTEM INTEGRATION ---
                // Notify your level manager that Stage 3 objective requirements are fulfilled
                SecondGenerator generator = Object.FindAnyObjectByType<SecondGenerator>();
                if (generator != null)
                {
                    // If your generator script has a state variable or method, update it here.
                    // For example: generator.isStageObjectiveComplete = true;
                    Debug.Log("[Stage 3 Key] LevelGenerator updated: Objective Secured. Extraction point accessible.");
                }
                else
                {
                    Debug.LogWarning("[Stage 3 Key] LevelGenerator not found in the active scene. Progression might not track!");
                }

                // Delete the physical item sprite from the map layout
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("[Stage 3 Key] Inventory full! Clear a slot to secure the objective key.");
            }
        }
        else
        {
            Debug.LogError("[Stage 3 Key] InventoryManager instance not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            localPlayer = other.GetComponent<PlayerController>();
            Debug.Log($"Press 'E' to secure the Stage 3 Key Item: {Stage3_Item?.itemName}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            localPlayer = null;
        }
    }
}