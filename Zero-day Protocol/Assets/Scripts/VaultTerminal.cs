using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class VaultTerminal : MonoBehaviour
{
    [Header("Linked UI Systems")]
    [Tooltip("Drag the global IPStream_Canvas game object here.")]
    public GameObject ipStreamCanvas;

    [Header("Settings")]
    public float hackDelaySeconds = 2.0f;
    public KeyCode interactionKey = KeyCode.E;

    private Slider worldProgressBar;
    private bool isPlayerNearby = false;
    private bool isHackingInProgress = false;
    private bool isHacked = false;

    void Start()
    {
        // Find the slider bar we just built inside our child components
        worldProgressBar = GetComponentInChildren<Slider>(true);
        if (worldProgressBar != null)
        {
            worldProgressBar.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNearby && !isHacked && !isHackingInProgress && Input.GetKeyDown(interactionKey))
        {
            StartCoroutine(HoldTimerRoutine());
        }
    }

    private IEnumerator HoldTimerRoutine()
    {
        isHackingInProgress = true;

        if (worldProgressBar != null)
        {
            worldProgressBar.gameObject.SetActive(true);
            worldProgressBar.value = 0f;
        }

        float elapsedTime = 0f;
        while (elapsedTime < hackDelaySeconds)
        {
            elapsedTime += Time.deltaTime;
            if (worldProgressBar != null)
            {
                worldProgressBar.value = elapsedTime / hackDelaySeconds;
            }
            yield return null;
        }

        if (worldProgressBar != null)
        {
            worldProgressBar.gameObject.SetActive(false);
        }

        LaunchPuzzleUI();
    }

    private void LaunchPuzzleUI()
    {
        if (ipStreamCanvas != null)
        {
            ipStreamCanvas.SetActive(true);

            var puzzleLogic = ipStreamCanvas.GetComponent<IPStreamPuzzle>();
            if (puzzleLogic != null)
            {
                puzzleLogic.LinkActiveTerminal(this);
            }
        }
        else
        {
            Debug.LogError("No IP Puzzle Canvas assigned to this terminal!");
            OnPuzzleComplete();
        }
    }

    public void OnPuzzleComplete()
    {
        isHacked = true;
        isHackingInProgress = false;
        Debug.Log("Terminal system fully bypassed!");
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
            if (isHackingInProgress)
            {
                StopAllCoroutines();
                isHackingInProgress = false;
                if (worldProgressBar != null) worldProgressBar.gameObject.SetActive(false);
            }
        }
    }
}