using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public string gunItemName = "Pistol";

    private InventoryManager inv;
    private Vector2 lastFacingDir = Vector2.up;

    void Start()
    {
        inv = Object.FindAnyObjectByType<InventoryManager>();

        // Resets ammo at the start of the level so the gun is always ready
        if (inv != null)
        {
            foreach (ItemData item in inv.itemSlots)
            {
                if (item != null && item.itemName == gunItemName)
                {
                    item.currentAmmo = item.maxAmmo;
                }
            }
        }
    }

    void Update()
    {
        UpdateFacingDirection();

        // Left Click to fire
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (IsGunSelected())
            {
                Shoot();
            }
        }
    }

    bool IsGunSelected()
    {
        if (inv == null) return false;

        // Ensure selectedSlot is valid
        if (inv.selectedSlot < 0 || inv.selectedSlot >= inv.itemSlots.Length) return false;

        ItemData selectedItem = inv.itemSlots[inv.selectedSlot];

        // Check if holding the gun and if it has bullets
        if (selectedItem != null && selectedItem.itemName == gunItemName)
        {
            if (selectedItem.currentAmmo > 0)
            {
                return true;
            }
            else
            {
                Debug.Log("Out of Ammo!");
                return false;
            }
        }
        return false;
    }

    void Shoot()
    {
        ItemData gunData = inv.itemSlots[inv.selectedSlot];
        if (gunData == null) return;

        // 1. Spend the bullet
        gunData.currentAmmo--;
        Debug.Log("Ammo left: " + gunData.currentAmmo);

        // 2. Create the bullet
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        // 3. Handle weapon break/discard
        if (gunData.currentAmmo <= 0)
        {
            Debug.Log("Gun empty! Removing from inventory.");

            // Clear the data from the manager
            inv.itemSlots[inv.selectedSlot] = null;

            // SYNC UI: This tells the InventoryManager to hide the icon
            // Make sure your InventoryManager has a function with this EXACT name
            inv.UpdateInventoryUI();
        }
    }

    void UpdateFacingDirection()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector2 moveInput = new Vector2(h, v);

        if (moveInput.sqrMagnitude > 0.01f)
        {
            lastFacingDir = moveInput.normalized;
            float angle = Mathf.Atan2(lastFacingDir.y, lastFacingDir.x) * Mathf.Rad2Deg - 90f;

            firePoint.rotation = Quaternion.Euler(0, 0, angle);
            firePoint.localPosition = lastFacingDir * 0.7f;
        }
    }
}