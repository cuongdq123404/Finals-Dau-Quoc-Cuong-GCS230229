using UnityEngine;

public class HubInteraction : MonoBehaviour
{
    public GameObject missionMenuUI; // Drag your UI Panel here
    private bool playerZone = false;

    void Start()
    {
        if (missionMenuUI != null)
            missionMenuUI.SetActive(false);
    }

    void Update()
    {
        // Check if the player is in the zone and presses E
        if (playerZone && Input.GetKeyDown(KeyCode.E))
        {
            if (missionMenuUI != null)
            {
                // Toggle the menu active state (On/Off)
                bool isCurrentlyActive = missionMenuUI.activeSelf;
                missionMenuUI.SetActive(!isCurrentlyActive);
                Debug.Log("Menu toggled! New state: " + !isCurrentlyActive);
            }
            else
            {
                Debug.LogError("HUB ERROR: MissionMenuUI is missing from the script slot in the Inspector!");
            }
        }
    }

    // --- TRIGGER DETECTION (For your Is Trigger collider) ---
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerZone = true;
            Debug.Log("Player entered the mission zone! Press E to open menu.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerZone = false;
            Debug.Log("Player left the mission zone.");
            if (missionMenuUI != null) missionMenuUI.SetActive(false);
        }
    }

    // --- PHYSICAL COLLISION DETECTION (Backup safety check) ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerZone = true;
            Debug.Log("Player is touching the terminal wall! Press E to open menu.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerZone = false;
            if (missionMenuUI != null) missionMenuUI.SetActive(false);
        }
    }
}