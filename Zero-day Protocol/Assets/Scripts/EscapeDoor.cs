using UnityEngine;
using UnityEngine.SceneManagement; // Crucial for loading scenes!

public class EscapeDoor : MonoBehaviour
{
    [Header("Door Settings")]
    public string requiredItemName = "keycard";
    public float interactionRange = 3.0f;
    public Sprite openDoorSprite;

    [Header("Tutorial Settings")]
    public bool isTutorialDoor = false; // Check this box ONLY in your Tutorial Scene!

    private bool isClosed = true;
    private Transform playerTransform;

    void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null)
        {
            playerTransform = p.transform;
        }
        else
        {
            Debug.LogError("DOOR ERROR: No object found with tag 'Player'! Check your Player's tag in the Inspector.");
        }
    }

    void Update()
    {
        if (!isClosed || playerTransform == null) return;

        float dist = Vector2.Distance(transform.position, playerTransform.position);

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (dist <= interactionRange)
            {
                TryOpenDoor();
            }
            else
            {
                Debug.Log("Too far from door! Distance: " + dist);
            }
        }
    }

    void TryOpenDoor()
    {
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();

        if (inv != null)
        {
            int keycardIndex = -1;

            for (int i = 0; i < inv.itemSlots.Length; i++)
            {
                if (inv.itemSlots[i] != null && inv.itemSlots[i].itemName.ToLower() == requiredItemName.ToLower())
                {
                    keycardIndex = i;
                    break;
                }
            }

            if (keycardIndex != -1)
            {
                // REMOVE ITEM
                inv.itemSlots[keycardIndex] = null;

                // CLEAN SYNC: Tell your inventory UI to refresh itself cleanly
                inv.UpdateInventoryUI();

                Open();
            }
            else
            {
                Debug.Log("Locked! You need: " + requiredItemName);
            }
        }
    }

    void Open()
    {
        isClosed = false;

        if (openDoorSprite != null)
        {
            GetComponent<SpriteRenderer>().sprite = openDoorSprite;
        }

        if (TryGetComponent(out Collider2D col))
        {
            col.enabled = false;
        }

        // SCENE MANAGEMENT SPLIT:
        if (isTutorialDoor)
        {
            Debug.Log("Tutorial Door Unlocked! Ending tutorial phase, returning to Main Menu.");
            SceneManager.LoadScene(0); // Index 0 is MainMenu
        }
        else
        {
            Debug.Log("Door Opened! Normal level escape successful.");
            SceneManager.LoadScene(2);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}