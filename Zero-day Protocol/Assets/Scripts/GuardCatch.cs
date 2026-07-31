using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardCatch : MonoBehaviour
{

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object we collided with has the Player tag
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player was caught by the guard! Resetting stage...");

            // Gets the currently active scene index and reloads it
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(currentSceneIndex);
        }
    }

    // Use this alternative if your Guard or Player uses "Is Trigger" on their collider
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player triggered guard detection! Resetting stage...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}