using UnityEngine;
using UnityEngine.UI; // Needed for UI Slider
using TMPro;          // Needed for TextMeshPro
using System.Collections;

public class CornerTerminal : MonoBehaviour
{
    [Header("Terminal Settings")]
    public int terminalID = 1;
    public float hackDelayDuration = 2.0f; // Fill duration in seconds

    [Header("UI Drag & Drop")]
    public Slider progressBarSlider;             // Drag your UI Slider here!
    public TextMeshProUGUI progressText;         // Optional: Drag "Hacking..." text here
    public GameObject interactionPromptUI;      // Optional: "Press [E]" prompt

    [Header("References")]
    public WaveHackQuizUI waveQuizUI;
    public ItemData usbItemData;

    private bool isPlayerInRange = false;
    private bool isHacked = false;
    private bool isHackingInProgress = false;
    private Coroutine hackCoroutine;

    void Start()
    {
        // Ensure slider is hidden on start
        if (progressBarSlider != null) progressBarSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isHacked || !isPlayerInRange || isHackingInProgress) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            StartHackSequence();
        }
    }

    void StartHackSequence()
    {
        isHackingInProgress = true;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        if (progressBarSlider != null)
        {
            hackCoroutine = StartCoroutine(FillProgressBar());
        }
        else
        {
            OnProgressBarComplete(); // Fallback if no Slider assigned
        }
    }

    IEnumerator FillProgressBar()
    {
        progressBarSlider.gameObject.SetActive(true);
        float timer = 0f;
        progressBarSlider.value = 0f;

        while (timer < hackDelayDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / hackDelayDuration);

            progressBarSlider.value = progress;

            if (progressText != null)
            {
                progressText.text = $"CONNECTING... {Mathf.RoundToInt(progress * 100)}%";
            }

            yield return null;
        }

        progressBarSlider.value = 1f;
        yield return new WaitForSeconds(0.15f);

        progressBarSlider.gameObject.SetActive(false);
        OnProgressBarComplete();
    }

    void OnProgressBarComplete()
    {
        isHackingInProgress = false;

        // Open the wave minigame
        if (waveQuizUI != null)
        {
            waveQuizUI.OpenWaveHack(OnWaveHackSuccess);
        }
    }

    void OnWaveHackSuccess()
    {
        isHacked = true;
        Debug.Log($"[TERMINAL {terminalID}] Hack Complete!");

        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        if (inv != null && usbItemData != null)
        {
            inv.AddItem(usbItemData);
        }

        BossRoomManager bossManager = Object.FindAnyObjectByType<BossRoomManager>();
        if (bossManager != null)
        {
            bossManager.OnTerminalHacked(terminalID);
        }

        this.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isHacked) return;
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

            // Cancel progress if player walks away mid-hack
            if (isHackingInProgress)
            {
                if (hackCoroutine != null) StopCoroutine(hackCoroutine);
                if (progressBarSlider != null) progressBarSlider.gameObject.SetActive(false);
                isHackingInProgress = false;
            }
        }
    }
}