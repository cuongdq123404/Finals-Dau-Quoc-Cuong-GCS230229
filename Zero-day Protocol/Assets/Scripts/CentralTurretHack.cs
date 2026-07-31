using UnityEngine;

public class CentralTurretHack : MonoBehaviour
{
    [Header("References")]
    public BossRoomManager bossManager;
    public ItemData usbItemData; // Drag your SecurityUSB asset here
    public int requiredUSBs = 4;

    [Header("Existing Turret Script Reference")]
    // Drag your actual combat/attack script component here in the Inspector
    public MonoBehaviour turretAttackScript;

    private bool isPlayerInRange = false;
    private bool isHacked = false;

    void Update()
    {
        if (isHacked || !isPlayerInRange) return;

        // Player presses 'E' near the turret
        if (Input.GetKeyDown(KeyCode.E))
        {
            TryHackCentralTurret();
        }
    }

    void TryHackCentralTurret()
    {
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        int usbCount = GetUSBCount(inv);

        if (usbCount < requiredUSBs)
        {
            int remaining = requiredUSBs - usbCount;
            Debug.Log($"[TURRET FIREWALL ACTIVE] Access Denied! You have {usbCount}/4 Security USBs. Need {remaining} more!");
        }
        else
        {
            ExecuteTurretShutdown();
        }
    }

    void ExecuteTurretShutdown()
    {
        isHacked = true;
        Debug.Log("[TURRET OVERRIDDEN] All 4 USB Keys accepted! Shutting down central power...");

        // 1. Disable your existing attack/combat script immediately so it stops firing!
        if (turretAttackScript != null)
        {
            turretAttackScript.enabled = false;
        }

        // 2. Notify BossRoomManager to trigger stage win (stun Warden, open exit door)
        if (bossManager != null)
        {
            bossManager.TriggerStageWin();
        }

        // 3. Disable this interaction script
        this.enabled = false;
    }

    int GetUSBCount(InventoryManager inv)
    {
        if (inv == null || inv.itemSlots == null) return 0;

        int count = 0;
        foreach (ItemData item in inv.itemSlots)
        {
            // Compares the item in slot directly with your USB ScriptableObject asset
            if (item != null && item == usbItemData)
            {
                count++;
            }
        }
        return count;
    }

    // --- Player Range Trigger ---
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInRange = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player")) isPlayerInRange = false;
    }
}