using UnityEngine;
using UnityEngine.UI; // For the UI Slider
using System.Collections;

public class HackerTerminal : MonoBehaviour
{
    [Header("Hacking Settings")]
    public float channelTime = 2.0f;
    public float blindDuration = 5.0f;
    public Slider progressBar;
    public GameObject puzzleUIPanel;

    private bool playerInRange = false;
    private bool isHacking = false;
    private bool isSolved = false;

    // Track the player controller component dynamically
    private PlayerController localPlayerController;

    void Start()
    {
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);
    }

    void Update()
    {
        if (playerInRange && !isHacking && !isSolved && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartHackingProcess());
        }
    }

    IEnumerator StartHackingProcess()
    {
        isHacking = true;
        float timer = 0f;

        // FREEZE THE PLAYER HERE
        if (localPlayerController != null)
        {
            localPlayerController.isFrozen = true;
        }

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
            progressBar.value = 0f;
        }

        while (timer < channelTime)
        {
            timer += Time.deltaTime;
            if (progressBar != null) progressBar.value = timer / channelTime;
            yield return null;
        }

        if (progressBar != null) progressBar.gameObject.SetActive(false);

        OpenPuzzleMiniGame();
    }

    void OpenPuzzleMiniGame()
    {
        if (puzzleUIPanel != null)
        {
            puzzleUIPanel.SetActive(true);
            AuditionPuzzle auditionScript = puzzleUIPanel.GetComponent<AuditionPuzzle>();
            if (auditionScript != null) auditionScript.Setup(this);
        }
    }

    public void OnPuzzleSolved()
    {
        isSolved = true;
        isHacking = false;

        if (puzzleUIPanel != null) puzzleUIPanel.SetActive(false);

        // UNFREEZE THE PLAYER HERE
        if (localPlayerController != null)
        {
            localPlayerController.isFrozen = false;
        }

        ApplyHackEffect();
    }

    void ApplyHackEffect()
    {
        // Using the updated, warning-free FindObjectsByType standard
        SimpleGuard[] allGuards = Object.FindObjectsByType<SimpleGuard>(FindObjectsSortMode.None);
        foreach (SimpleGuard guard in allGuards)
        {
            if (guard != null) StartCoroutine(BlindGuardSequence(guard));
        }
    }

    IEnumerator BlindGuardSequence(SimpleGuard guard)
    {
        float originalViewDist = guard.viewDist;
        guard.viewDist = 0f;

        Transform coneVisual = null;
        if (guard.visionPivot != null)
        {
            coneVisual = guard.visionPivot.Find("Triangle");
            if (coneVisual != null) coneVisual.gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(blindDuration);

        if (guard != null)
        {
            guard.viewDist = originalViewDist; // Fixed variable name mapping here
            if (coneVisual != null) coneVisual.gameObject.SetActive(true);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;

            // Correctly fetching your custom PlayerController script
            localPlayerController = other.GetComponent<PlayerController>();

            Debug.Log("Press 'E' to begin hacking terminal.");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;

            // Safety wipe if they somehow glitch away
            if (!isHacking) localPlayerController = null;
        }
    }
}