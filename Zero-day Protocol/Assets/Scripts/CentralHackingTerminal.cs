using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CentralHackingTerminal : MonoBehaviour
{
    [Header("Terminal Settings")]
    public float overrideDuration = 2.0f;
    public int requiredUSBs = 4;

    [Header("UI Drag & Drop")]
    public Slider progressBarSlider;
    public TextMeshProUGUI progressText;
    public GameObject interactionPromptUI;

    [Header("References")]
    public BossRoomManager bossManager;
    public ExitDoor exitDoorScript; // Drag your Exit Door GameObject here!
    public ItemData usbItemData;

    private bool isPlayerInRange = false;
    private bool isHacked = false;
    private bool isHackingInProgress = false;
    private Coroutine hackCoroutine;

    void Start()
    {
        if (progressBarSlider != null) progressBarSlider.gameObject.SetActive(false);
    }

    void Update()
    {
        if (isHacked || !isPlayerInRange || isHackingInProgress) return;

        if (Input.GetKeyDown(KeyCode.E))
        {
            TryStartCentralHack();
        }
    }

    void TryStartCentralHack()
    {
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        int usbCount = GetUSBCount(inv);

        if (usbCount < requiredUSBs)
        {
            int remaining = requiredUSBs - usbCount;
            Debug.Log($"[ACCESS DENIED] Need {remaining} more USB key(s)!");
            return;
        }

        isHackingInProgress = true;
        if (interactionPromptUI != null) interactionPromptUI.SetActive(false);

        if (progressBarSlider != null)
        {
            hackCoroutine = StartCoroutine(FillProgressBar());
        }
        else
        {
            OnOverrideComplete();
        }
    }

    IEnumerator FillProgressBar()
    {
        progressBarSlider.gameObject.SetActive(true);
        float timer = 0f;
        progressBarSlider.value = 0f;

        while (timer < overrideDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / overrideDuration);

            progressBarSlider.value = progress;

            if (progressText != null)
            {
                progressText.text = $"OVERRIDING SYSTEM... {Mathf.RoundToInt(progress * 100)}%";
            }

            yield return null;
        }

        progressBarSlider.value = 1f;
        yield return new WaitForSeconds(0.15f);

        progressBarSlider.gameObject.SetActive(false);
        OnOverrideComplete();
    }

    void OnOverrideComplete()
    {
        isHacked = true;
        isHackingInProgress = false;

        // 1. CLEAR ALL USBs FROM INVENTORY
        InventoryManager inv = Object.FindAnyObjectByType<InventoryManager>();
        if (inv != null && usbItemData != null)
        {
            inv.RemoveAllItemsOfType(usbItemData);
        }

        // 2. Notify BossRoomManager to shutdown Warden/Turret
        if (bossManager != null)
        {
            bossManager.ExecutePhase4_AutoShutdown();
        }

        // 3. Unlock the Exit Door without destroying/deactivating it
        if (exitDoorScript != null)
        {
            exitDoorScript.UnlockDoor();
        }
        else
        {
            Debug.LogWarning("[CENTRAL TERMINAL] ExitDoor reference is missing in the Inspector!");
        }

        this.enabled = false;
    }

    int GetUSBCount(InventoryManager inv)
    {
        if (inv == null || inv.itemSlots == null) return 0;

        int count = 0;
        foreach (ItemData item in inv.itemSlots)
        {
            if (item != null && item == usbItemData) count++;
        }
        return count;
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

            if (isHackingInProgress)
            {
                if (hackCoroutine != null) StopCoroutine(hackCoroutine);
                if (progressBarSlider != null) progressBarSlider.gameObject.SetActive(false);
                isHackingInProgress = false;
            }
        }
    }
}