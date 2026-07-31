using UnityEngine;

public class InteractableChest : MonoBehaviour
{
    [Header("Visual Sprites")]
    public SpriteRenderer spriteRenderer;
    public Sprite openChestSprite;

    [Header("Inventory Item Settings")]
    public ItemData weaponItem; // Assign an ItemData asset (e.g. Pistol) in the inspector

    [Header("Chest State")]
    private bool isPlayerInRange = false;
    private bool isOpened = false;

    // Store a reference to the player controller who walked into our trigger zone
    private PlayerController localPlayer;

    void Update()
    {
        // If the player is standing next to the chest, it's closed, and they hit E
        if (isPlayerInRange && !isOpened && Input.GetKeyDown(KeyCode.E) && localPlayer != null)
        {
            OpenChest();
        }
    }

    void OpenChest()
    {
        isOpened = true;

        // 1. Swap the visual sprite from closed to open
        if (spriteRenderer != null && openChestSprite != null)
        {
            spriteRenderer.sprite = openChestSprite;
        }

        // 2. Safely find the inventory component sitting on your player controller
        InventoryManager playerInventory = localPlayer.GetComponent<InventoryManager>();

        if (playerInventory != null)
        {
            if (weaponItem != null)
            {
                // --- FIX: RESET AMMO ON PICKUP ---
                // Reset the ScriptableObject ammo so it always starts at max when taken out of the chest
                weaponItem.currentAmmo = weaponItem.maxAmmo;

                // 3. Inject the ItemData into their inventory backend
                bool added = playerInventory.AddItem(weaponItem);
                if (added)
                    Debug.Log($"[Chest] Opened! Added {weaponItem.itemName} to the player's inventory.");
                else
                    Debug.LogWarning("[Chest] Inventory was full, could not add the item.");
            }
            else
            {
                Debug.LogError("[Chest] 'weaponItem' is not assigned. Set an ItemData asset in the inspector.");
            }
        }
        else
        {
            Debug.LogError("[Chest] Found the player, but couldn't find an 'InventoryManager' script component on them!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            localPlayer = other.GetComponent<PlayerController>(); // Cache the player reference
            Debug.Log("Press 'E' to open weapon chest.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            localPlayer = null; // Wipe the cache when they walk away
        }
    }
}