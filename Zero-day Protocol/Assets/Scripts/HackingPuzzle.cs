using UnityEngine;
using TMPro; // Required for TextMeshPro text manipulation
using System.Collections.Generic;

public class AuditionPuzzle : MonoBehaviour
{
    [Header("UI Display References")]
    public TextMeshProUGUI sequenceText;
    public TextMeshProUGUI statusText; // Shows "Bypassing...", "FAILED", "SUCCESS"

    [Header("Puzzle Tuning")]
    public int sequenceLength = 4; // Number of commands the player has to press

    private HackerTerminal activeTerminal;

    // Internal tracking of the puzzle steps
    private List<string> generatedSequence = new List<string>();
    private int currentInputIndex = 0;
    private bool isPuzzleActive = false;

    // Called automatically by the HackerTerminal when the 2-second wait finishes
    public void Setup(HackerTerminal terminal)
    {
        activeTerminal = terminal;
        currentInputIndex = 0;
        isPuzzleActive = true;

        if (statusText != null) statusText.text = "OVERRIDE SYSTEM INITIALIZED...";

        GenerateRandomSequence();
        UpdateDisplayUI();
    }

    void GenerateRandomSequence()
    {
        generatedSequence.Clear();
        string[] arrowPool = { "Up", "Down", "Left", "Right" };

        for (int i = 0; i < sequenceLength; i++)
        {
            int randomIndex = Random.Range(0, arrowPool.Length);
            generatedSequence.Add(arrowPool[randomIndex]);
        }
    }

    void UpdateDisplayUI()
    {
        if (sequenceText == null) return;

        string displayString = "";

        for (int i = 0; i < generatedSequence.Count; i++)
        {
            string arrowSymbol = "";

            // Map the internal strings to clean visual symbols
            switch (generatedSequence[i])
            {
                case "Up": arrowSymbol = "▲"; break;
                case "Down": arrowSymbol = "▼"; break;
                case "Left": arrowSymbol = "◄"; break;
                case "Right": arrowSymbol = "►"; break;
            }

            // Highlight the arrows the player has already successfully pressed green!
            if (i < currentInputIndex)
            {
                displayString += $"<color=green>{arrowSymbol}</color>   ";
            }
            else
            {
                displayString += $"<color=white>{arrowSymbol}</color>   ";
            }
        }

        sequenceText.text = displayString;
    }

    void Update()
    {
        if (!isPuzzleActive) return;

        // 1. If the player has pressed all directional inputs, they MUST slam Spacebar to execute
        if (currentInputIndex >= generatedSequence.Count)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                HandleSuccess();
            }
            return;
        }

        // 2. Otherwise, check for the next arrow input in line
        string expectedInput = generatedSequence[currentInputIndex];

        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            CheckPlayerInput("Up", expectedInput);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
        {
            CheckPlayerInput("Down", expectedInput);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            CheckPlayerInput("Left", expectedInput);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            CheckPlayerInput("Right", expectedInput);
        }
    }

    void CheckPlayerInput(string inputPressed, string expectedInput)
    {
        if (inputPressed == expectedInput)
        {
            currentInputIndex++;
            UpdateDisplayUI();
        }
        else
        {
            // If they miss a single note, it resets back to the beginning of the stream!
            HandleFailure();
        }
    }

    void HandleFailure()
    {
        Debug.LogWarning("[Hacking] Input mismatch! Sequence corrupted. Resetting sequence...");
        currentInputIndex = 0; // Reset progress counter
        if (statusText != null) statusText.text = "<color=red>SIGNAL CORRUPTED! RESTARTING...</color>";
        UpdateDisplayUI();
    }

    void HandleSuccess()
    {
        isPuzzleActive = false;
        if (statusText != null) statusText.text = "<color=green>ACCESS GRANTED!</color>";
        Debug.Log("[Hacking] Audition override protocol successful!");

        if (activeTerminal != null)
        {
            activeTerminal.OnPuzzleSolved();
        }
    }
}