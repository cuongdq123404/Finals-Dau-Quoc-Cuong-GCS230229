using UnityEngine;
using UnityEngine.SceneManagement; // Required for reloading the level

public class LaserHazard : MonoBehaviour
{
    [Header("Laser Settings")]
    [Tooltip("Make sure your Player GameObject has the tag 'Player' in the Inspector!")]
    public string playerTag = "Player";

    // This function automatically runs when a 2D collider enters this object's trigger zone
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that touched the laser is the player
        if (other.CompareTag(playerTag))
        {
            Debug.Log("Player fried by security laser! Restarting stage...");

            RestartCurrentStage();
        }
    }

    private void RestartCurrentStage()
    {
        // 1. Get the build index of the scene you are currently playing
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        // 2. Reload that exact scene from scratch
        SceneManager.LoadScene(currentSceneIndex);
    }
}