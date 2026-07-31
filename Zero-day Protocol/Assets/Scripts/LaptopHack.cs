using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class LaptopHack : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider progressBar;
    public GameObject minigamePanel;
    public Slider hackSlider;
    public Transform playerTransform;
    public TextMeshProUGUI feedbackText;

    [Header("Chest Connection")]
    public Chest targetChest; // Drag your Chest object here!

    [Header("Minigame Tuning")]
    public float sliderSpeed = 2.5f;
    public float targetMin = 40f;
    public float targetMax = 60f;
    public int requiredHits = 3;
    public int maxMisses = 5;
    public float timeLimit = 15f;

    [Header("Status (For Guard AI)")]
    public bool isPlayerHacking = false;

    private int currentHits = 0;
    private int currentMisses = 0;
    private float currentTime;
    private bool canHack = false;
    private bool isMinigameActive = false;
    private bool isAlreadyHacked = false;

    void Start()
    {
        if (minigamePanel != null) minigamePanel.SetActive(false);
        if (progressBar != null) progressBar.gameObject.SetActive(false);
        if (feedbackText != null) feedbackText.text = "";
    }

    void Update()
    {
        // Start hack if player is in range and presses E
        if (canHack && Input.GetKeyDown(KeyCode.E) && !isMinigameActive && !isAlreadyHacked)
        {
            StartCoroutine(StartLoadingBar());
        }

        if (isMinigameActive)
        {
            currentTime -= Time.deltaTime;

            // UI follows player or stays fixed
            minigamePanel.transform.position = playerTransform.position + new Vector3(0, 1.5f, 0);
            hackSlider.value = Mathf.PingPong(Time.time * 50 * sliderSpeed, 100);

            UpdateTimerUI();

            if (currentTime <= 0)
            {
                StartCoroutine(HandleFinalFailure("TIME EXPIRED!"));
                return;
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                CheckTiming();
            }
        }
    }

    IEnumerator StartLoadingBar()
    {
        // Freeze movement
        if (playerTransform.TryGetComponent(out PlayerController moveScript)) moveScript.enabled = false;

        currentMisses = 0;
        currentHits = 0;
        currentTime = timeLimit;
        isPlayerHacking = true;

        progressBar.gameObject.SetActive(true);
        float t = 0;
        while (t < 2f)
        {
            t += Time.deltaTime;
            progressBar.value = (t / 2f) * 100;
            yield return null;
        }
        progressBar.gameObject.SetActive(false);

        isMinigameActive = true;
        minigamePanel.SetActive(true);
    }

    void UpdateTimerUI()
    {
        if (feedbackText != null)
        {
            feedbackText.text = $"TIME: {currentTime:F1}s\nHITS: {currentHits}/{requiredHits}";
        }
    }

    void CheckTiming()
    {
        if (hackSlider.value >= targetMin && hackSlider.value <= targetMax)
        {
            currentHits++;
            UpdateFeedback("DIRECT HIT!", Color.green);
            if (currentHits >= requiredHits) StartCoroutine(HandleWin());
        }
        else
        {
            currentMisses++;
            currentHits = 0; // Reset streak on miss
            UpdateFeedback("TRACED! " + currentMisses + "/" + maxMisses, Color.red);
            if (currentMisses >= maxMisses) StartCoroutine(HandleFinalFailure("SYSTEM LOCKED!"));
        }
    }

    void UpdateFeedback(string msg, Color col)
    {
        StopCoroutine("TempFeedback");
        StartCoroutine(TempFeedback(msg, col));
    }

    IEnumerator TempFeedback(string msg, Color col)
    {
        feedbackText.text = msg;
        feedbackText.color = col;
        yield return new WaitForSeconds(0.5f);
        feedbackText.color = Color.white;
    }

    IEnumerator HandleWin()
    {
        isMinigameActive = false;
        isAlreadyHacked = true;
        isPlayerHacking = false;

        feedbackText.text = "ACCESS GRANTED!";
        feedbackText.color = Color.green;
        yield return new WaitForSeconds(1f);

        // --- THE CRITICAL CONNECTION ---
        if (targetChest != null)
        {
            targetChest.RemoteUnlock(); // This flips isLocked to false in Chest.cs
        }

        // Visual feedback on the laptop itself
        GetComponent<SpriteRenderer>().color = Color.green;

        UnfreezePlayer();
        CloseMenu();
    }

    IEnumerator HandleFinalFailure(string reason)
    {
        isMinigameActive = false;
        isPlayerHacking = false;

        feedbackText.text = reason;
        feedbackText.color = Color.red;
        yield return new WaitForSeconds(1.5f);

        UnfreezePlayer();
        CloseMenu();
    }

    void UnfreezePlayer()
    {
        if (playerTransform.TryGetComponent(out PlayerController moveScript)) moveScript.enabled = true;
    }

    public void CloseMenu()
    {
        isMinigameActive = false;
        minigamePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other) { if (other.CompareTag("Player")) canHack = true; }
    private void OnTriggerExit2D(Collider2D other) { if (other.CompareTag("Player")) canHack = false; }
}