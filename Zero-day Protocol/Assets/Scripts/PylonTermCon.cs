using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PylontermCon : MonoBehaviour
{
    [Header("Linked Security Systems")]
    public MagneticPylon linkedPylon;
    public GameObject linkedExitDoor;

    [Header("UI Canvas Elements")]
    public GameObject hackingUIPanel;      // The main background panel
    public Slider progressBar;              // The loading/progress slider
    public TextMeshProUGUI targetKeyText;   // Text component for display messages

    [Header("Boot-up Settings")]
    public float connectionDelay = 2.0f;    // 2-second boot up time

    [Header("Quiz Settings")]
    [Range(0.1f, 1f)] public float progressGainPerPress = 0.25f;
    public float progressDrainRate = 0.05f;

    private bool playerInRange = false;
    private bool isHacking = false;
    private bool isBootingUp = false;
    private bool isSolved = false;

    private char targetKey;
    private char[] possibleKeys = { 'W', 'A', 'S', 'D' };
    private PlayerController localPlayerController;

    void Start()
    {
        if (hackingUIPanel != null) hackingUIPanel.SetActive(false);
    }

    void Update()
    {
        if (isSolved) return;

        // Start hacking when pressing E near the terminal
        if (playerInRange && !isHacking && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(StartHackingProcess());
        }

        // Handle the keyboard game input only after boot-up finishes
        if (isHacking && !isBootingUp)
        {
            HandlePuzzleInput();
        }
    }

    IEnumerator StartHackingProcess()
    {
        isHacking = true;
        isBootingUp = true;
        float timer = 0f;

        // FREEZE THE PLAYER (Matches your preferred design style!)
        if (localPlayerController != null)
        {
            localPlayerController.isFrozen = true;

            // Optional structural check: stop player sliding if they have velocity
            Rigidbody2D playerRb = localPlayerController.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.linearVelocity = Vector2.zero;
                playerRb.bodyType = RigidbodyType2D.Kinematic; // Shield from repulsor force
            }
        }

        if (hackingUIPanel != null) hackingUIPanel.SetActive(true);
        if (targetKeyText != null) targetKeyText.text = "CONNECTING TO NETWORK...";

        // Stage 1: The 2-second boot-up channel
        while (timer < connectionDelay)
        {
            timer += Time.deltaTime;
            if (progressBar != null) progressBar.value = timer / connectionDelay;
            yield return null;
        }

        // Reset progress bar to empty for the actual puzzle keys
        if (progressBar != null) progressBar.value = 0f;

        isBootingUp = false;
        GenerateNewTargetKey(); // Move to the WASD puzzle phase
    }

    void HandlePuzzleInput()
    {
        // Slowly drain progress over time to add tension
        if (progressBar != null && progressBar.value > 0)
        {
            progressBar.value -= progressDrainRate * Time.deltaTime;
        }

        // Detect player keystrokes
        if (Input.anyKeyDown)
        {
            string inputString = Input.inputString.ToUpper();
            if (inputString.Length > 0)
            {
                char pressedChar = inputString[0];

                if (pressedChar == targetKey)
                {
                    progressBar.value += progressGainPerPress;

                    if (progressBar.value >= 1f)
                    {
                        OnPuzzleSolved();
                    }
                    else
                    {
                        GenerateNewTargetKey();
                    }
                }
            }
        }
    }

    void GenerateNewTargetKey()
    {
        int randomIndex = Random.Range(0, possibleKeys.Length);
        targetKey = possibleKeys[randomIndex];

        if (targetKeyText != null)
        {
            targetKeyText.text = $"ENTER DECRYPTION KEY: [ {targetKey} ]";
        }
    }

    public void OnPuzzleSolved()
    {
        isSolved = true;
        isHacking = false;

        if (hackingUIPanel != null) hackingUIPanel.SetActive(false);

        // UNFREEZE THE PLAYER
        if (localPlayerController != null)
        {
            localPlayerController.isFrozen = false;

            Rigidbody2D playerRb = localPlayerController.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                playerRb.bodyType = RigidbodyType2D.Dynamic; // Restore physics controls
            }
        }

        // Apply Level Effects directly inside this room script
        if (linkedPylon != null)
        {
            linkedPylon.isActive = false;
        }

        if (linkedExitDoor != null)
        {
            linkedExitDoor.SetActive(false); // Hide the exit door gameobject completely
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            localPlayerController = other.GetComponent<PlayerController>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (isHacking)
            {
                CancelHack();
            }
            else
            {
                localPlayerController = null;
            }
        }
    }

    private void CancelHack()
    {
        StopAllCoroutines();
        isHacking = false;
        isBootingUp = false;

        if (hackingUIPanel != null) hackingUIPanel.SetActive(false);

        if (localPlayerController != null)
        {
            localPlayerController.isFrozen = false;
            Rigidbody2D playerRb = localPlayerController.GetComponent<Rigidbody2D>();
            if (playerRb != null) playerRb.bodyType = RigidbodyType2D.Dynamic;
        }

        localPlayerController = null;
    }
}