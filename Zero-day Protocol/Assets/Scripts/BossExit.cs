using UnityEngine;
using UnityEngine.SceneManagement; // Useful if you want to load the next level!

public class ExitDoor : MonoBehaviour
{
    [Header("Settings")]
    public string nextSceneName = "WinScene"; // Or leave blank if handling transition elsewhere
    public GameObject interactionPromptUI;   // Optional: "Press [F] to Escape" prompt

    private bool isUnlocked = false;
    private bool isPlayerInRange = false;

    void Start()
    {
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
    }

    void Update()
    {
        // Only allow interaction if the central hack unlocked the door AND player is in range
        if (!isUnlocked || !isPlayerInRange) return;

        if (Input.GetKeyDown(KeyCode.F))
        {
            Escape();
        }
    }

    /// <summary>
    /// Called by CentralHackingTerminal or BossRoomManager after turret hack completes
    /// </summary>
    public void UnlockDoor()
    {
        isUnlocked = true;
        Debug.Log("[EXIT DOOR] Unlocked! Player can now press 'F' to escape.");
    }

    void Escape()
    {
        Debug.Log("==========================================");
        Debug.Log("PLAYER ESCAPED! STAGE COMPLETE!");
        Debug.Log("==========================================");

        // Load next scene if scene name is specified
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;

            // Only show prompt if door is actually unlocked
            if (isUnlocked && interactionPromptUI != null)
            {
                interactionPromptUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(false);
        }
    }
}