using UnityEngine;

public class PhysicalKeycardItem : MonoBehaviour
{
    [Header("Item Config")]
    public ItemData keycardItem; // Assign the ItemData asset for this keycard in the Inspector

    private bool isPlayerInRange = false;
    private PlayerController localPlayer;

    void Update()
    {
        // When standing near the desk and hitting 'E', pick it up
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && localPlayer != null)
        {
            PickUpKeycard();
        }
    }

    void PickUpKeycard()
    {
        if (keycardItem == null)
        {
            Debug.LogError("[Pickup] keycardItem not set on PhysicalKeycardItem!");
            return;
        }

        // Use the project's InventoryManager (there is no InventoryController type)
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();

        if (inv != null)
        {
            bool added = inv.AddItem(keycardItem);
            if (added)
            {
                Debug.Log($"[Pickup] {keycardItem.itemName} added to inventory!");
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("[Pickup] Inventory full. Could not add item.");
            }
        }
        else
        {
            Debug.LogError("[Pickup] InventoryManager instance not found in scene!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            localPlayer = other.GetComponent<PlayerController>();
            Debug.Log("Press 'E' to pick up the Key Item.");
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