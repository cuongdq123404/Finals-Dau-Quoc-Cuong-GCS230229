using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour
{
    [Header("UI Layout")]
    public RectTransform[] slots;
    public RectTransform selector;

    [Header("Inventory Data")]
    public ItemData[] itemSlots = new ItemData[5];
    public Image[] iconImages;

    [Header("Ammo UI")]
    public TMPro.TextMeshProUGUI ammoText; // If using TextMeshPro, change this to: public TMPro.TextMeshProUGUI ammoText;

    public int selectedSlot = 0;

    void Update()
    {
        // 1. Slot Selection
        if (Input.GetKeyDown(KeyCode.Alpha1)) selectedSlot = 0;
        else if (Input.GetKeyDown(KeyCode.Alpha2)) selectedSlot = 1;
        else if (Input.GetKeyDown(KeyCode.Alpha3)) selectedSlot = 2;
        else if (Input.GetKeyDown(KeyCode.Alpha4)) selectedSlot = 3;
        else if (Input.GetKeyDown(KeyCode.Alpha5)) selectedSlot = 4;

        // 2. Scroll Wheel Support
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll > 0f) selectedSlot = (selectedSlot > 0) ? selectedSlot - 1 : 4;
        else if (scroll < 0f) selectedSlot = (selectedSlot < 4) ? selectedSlot + 1 : 0;

        UpdateSelectorPosition();
        UpdateAmmoDisplay(); // Keep the ammo count updated every frame
    }

    void UpdateSelectorPosition()
    {
        if (selector != null && slots.Length > selectedSlot)
        {
            selector.position = slots[selectedSlot].position;
        }
    }

    // Logic to show/hide and update the ammo numbers
    void UpdateAmmoDisplay()
    {
        if (ammoText == null) return;

        ItemData currentItem = itemSlots[selectedSlot];

        // Check if the item exists and is actually a weapon
        if (currentItem != null && currentItem.isWeapon)
        {
            ammoText.text = "Ammo: " + currentItem.currentAmmo + " / " + currentItem.maxAmmo;

            // UI Polish: Turn text red when on the last bullet
            ammoText.color = (currentItem.currentAmmo <= 1) ? Color.red : Color.white;
        }
        else
        {
            // Hide the text if we aren't holding a gun
            ammoText.text = "";
        }
    }

    public bool AddItem(ItemData newItem)
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] == null)
            {
                itemSlots[i] = newItem;
                UpdateInventoryUI();
                return true;
            }
        }
        return false;
    }

    public void UpdateInventoryUI()
    {
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (i < iconImages.Length && iconImages[i] != null)
            {
                if (itemSlots[i] == null)
                {
                    iconImages[i].sprite = null;
                    iconImages[i].enabled = false;
                }
                else
                {
                    iconImages[i].sprite = itemSlots[i].icon;
                    iconImages[i].enabled = true;
                }
            }
        }
    }

    public int GetCurrentSlot() => selectedSlot;
    public void RemoveAllItemsOfType(ItemData targetItem)
    {
        if (targetItem == null) return;

        // Iterate through all inventory slots and clear matching items
        for (int i = 0; i < itemSlots.Length; i++)
        {
            if (itemSlots[i] != null && itemSlots[i] == targetItem)
            {
                itemSlots[i] = null;
            }
        }

        UpdateInventoryUI();
        Debug.Log($"[INVENTORY] Cleared all copies of {targetItem.itemName} from inventory.");
    }
}
