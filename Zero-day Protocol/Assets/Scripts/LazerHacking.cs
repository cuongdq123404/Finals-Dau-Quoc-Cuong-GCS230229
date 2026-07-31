using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Required to use the Slider component

public class HackingLazer : MonoBehaviour
{
    [Header("Target Security Objects")]
    public GameObject targetLaserGate;

    [Header("Settings")]
    public float hackDelaySeconds = 2.0f;
    public KeyCode interactionKey = KeyCode.E;

    public GameObject hackingCanvasObject;
    private Slider progressBar; // Automatically found on the floating World Space Canvas
    private bool isPlayerNearby = false;
    private bool isHackingInProgress = false;
    private bool isHacked = false;

    void Update()
    {
        // Start hacking if player is nearby, not already hacking, and presses E
        if (isPlayerNearby && !isHacked && !isHackingInProgress && Input.GetKeyDown(interactionKey))
        {
            StartCoroutine(HackSequenceRoutine());
        }
    }

    private IEnumerator HackSequenceRoutine()
    {
        isHackingInProgress = true;
        Debug.Log("Siphoning data network... Hold position!");

        // 1. Find and setup the world-space progress bar on top of the terminal
        FindLocalProgressBar();
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }

        // 2. --- THE ANIMATED PROGRESS WAIT ---
        float elapsedTime = 0f;
        while (elapsedTime < hackDelaySeconds)
        {
            elapsedTime += Time.deltaTime;

            if (progressBar != null)
            {
                // Smoothly fill the bar from 0 to 1 based on time passed
                progressBar.value = elapsedTime / hackDelaySeconds;
            }

            yield return null; // Wait for the next frame
        }

        // 3. Turn off the floating progress bar when done filling
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        // 4. --- TRIGGER THE PUZZLE ---
        Debug.Log("Firewall breached. Launching bypass mini-game!");
        TriggerHackingPuzzle();
    }

    private void FindLocalProgressBar()
    {
        // Looks directly inside this terminal's children for the floating world-space slider
        if (progressBar == null)
        {
            progressBar = GetComponentInChildren<Slider>(true);
        }
    }

    private void TriggerHackingPuzzle()
    {

        if (hackingCanvasObject != null)
        {
            // Turn on the full-screen puzzle UI overlay
            hackingCanvasObject.SetActive(true);

            // Pass this script instance over to the puzzle logic controller
            var puzzleScript = hackingCanvasObject.GetComponent<LazerQuizController>();
            if (puzzleScript != null)
            {
                puzzleScript.currentLazer = this;
            }
        }
        else
        {
            Debug.LogError("Hacking Lazer script couldn't find 'Hacking_Canvas' in the scene! Defaulting to instant solve.");
            OnPuzzleSolved(); // Fallback safety so your project doesn't freeze if you forget the canvas
        }
    }

    // Call this public function from your specific Puzzle Script when the player wins!
    public void OnPuzzleSolved()
    {
        isHacked = true;
        isHackingInProgress = false;
        Debug.Log("Puzzle Decrypted! Access Granted.");

        if (targetLaserGate != null)
        {
            Destroy(targetLaserGate);
            Debug.Log("Target security grid is offline.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) isPlayerNearby = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // Interrupt the hack if the player panics and runs away mid-download!
            if (isHackingInProgress)
            {
                StopAllCoroutines();
                isHackingInProgress = false;

                // Hide the progress bar immediately on escape
                if (progressBar != null)
                {
                    progressBar.gameObject.SetActive(false);
                }

                Debug.Log("Hack aborted! Connection lost.");
            }
        }
    }
}