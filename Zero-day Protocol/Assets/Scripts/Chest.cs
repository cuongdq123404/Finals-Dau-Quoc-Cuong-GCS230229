using UnityEngine;
using System.Collections.Generic; // Required for Lists

public class Chest : MonoBehaviour
{
    [Header("Item Data")]
    // This allows you to add as many items as you want in the Inspector
    public List<ItemData> contents = new List<ItemData>();
    public Sprite openChestSprite;

    [Header("Settings")]
    public float interactionRange = 2.0f;
    public KeyCode interactKey = KeyCode.F;
    public bool isLocked = true; // Must be unlocked by LaptopHack

    private bool isOpened = false;
    private Transform player;

    void Start()
    {
        // Find the player automatically by tag
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (isOpened || player == null) return;

        float dist = Vector2.Distance(transform.position, player.position);

        if (dist <= interactionRange && Input.GetKeyDown(interactKey))
        {
            if (isLocked)
            {
                Debug.Log("Chest is electronically locked! Find the Laptop.");
                return;
            }

            OpenChest();
        }
    }

    // Called by the LaptopHack script
    public void RemoteUnlock()
    {
        isLocked = false;
        Debug.Log("Chest Unlocked via Hack!");
    }

    void OpenChest()
    {
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();

        if (inv != null)
        {
            // We use a temporary list because you cannot remove items from a 
            // list while you are looping (foreach) through it.
            List<ItemData> itemsToRemove = new List<ItemData>();

            foreach (ItemData item in contents)
            {
                if (inv.AddItem(item))
                {
                    Debug.Log("Looted: " + item.itemName);
                    itemsToRemove.Add(item);
                }
                else
                {
                    Debug.Log("Inventory full! Could not fit: " + item.itemName);
                }
            }

            // Remove only the items we actually picked up
            foreach (ItemData item in itemsToRemove)
            {
                contents.Remove(item);
            }

            // --- FIXED LINE BELOW (Matched to your variable name) ---
            if (contents.Count == 0)
            {
                isOpened = true;
                if (openChestSprite != null)
                {
                    GetComponent<SpriteRenderer>().sprite = openChestSprite;
                }

                // Disable the script so we stop checking distance/input
                this.enabled = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}