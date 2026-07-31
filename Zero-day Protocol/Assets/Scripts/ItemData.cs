using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    public Sprite icon;        // The sprite for the UI slot
    public bool isWeapon;      // Can we fire it?
    public GameObject prefab;  // The object to spawn in the world

    [Header("Gun Settings")]
    public int currentAmmo;
    public int maxAmmo = 6;

    // This runs automatically in the Unity Editor 
    // It ensures new guns start with full ammo by default
    private void OnValidate()
    {
        if (isWeapon && currentAmmo == 0)
        {
            currentAmmo = maxAmmo;
        }
    }
}