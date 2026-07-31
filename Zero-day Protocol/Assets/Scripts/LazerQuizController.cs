using UnityEngine;
using UnityEngine.UI;

public class LazerQuizController : MonoBehaviour
{
    [Header("Linked Game Systems")]
    public HackingLazer currentLazer;

    [Header("Puzzle Settings")]
    public Button[] puzzleNodesInOrder;

    private Button[] dynamicWinningOrder;
    private int currentStepIndex = 0;

    void OnEnable()
    {
        currentStepIndex = 0;
        GenerateRandomSequence();
        ResetNodeColors();
        ShowNextHint(); // Highlight the very first step!
    }

    private void GenerateRandomSequence()
    {
        if (puzzleNodesInOrder == null || puzzleNodesInOrder.Length == 0) return;

        dynamicWinningOrder = (Button[])puzzleNodesInOrder.Clone();

        for (int i = dynamicWinningOrder.Length - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            Button temp = dynamicWinningOrder[i];
            dynamicWinningOrder[i] = dynamicWinningOrder[randomIndex];
            dynamicWinningOrder[randomIndex] = temp;
        }
    }

    public void OnNodeClicked(Button clickedButton)
    {
        if (clickedButton == dynamicWinningOrder[currentStepIndex])
        {
            // Lock in the correct choice with Cyan
            clickedButton.image.color = Color.cyan;
            currentStepIndex++;

            if (currentStepIndex >= dynamicWinningOrder.Length)
            {
                WinHack();
            }
            else
            {
                // Show the next hint in the chain!
                ShowNextHint();
            }
        }
        else
        {
            Debug.LogWarning("ACCESS DENIED: Circuit broken. Resetting sequence!");
            currentStepIndex = 0;
            ResetNodeColors();
            ShowNextHint(); // Restart hints from the beginning
        }
    }

    private void ShowNextHint()
    {
        // Highlight the next correct button in Yellow so the player knows what to click
        if (dynamicWinningOrder != null && currentStepIndex < dynamicWinningOrder.Length)
        {
            // Don't overwrite the Red color if it's the final node
            if (currentStepIndex == dynamicWinningOrder.Length - 1)
            {
                dynamicWinningOrder[currentStepIndex].image.color = Color.red;
            }
            else
            {
                dynamicWinningOrder[currentStepIndex].image.color = Color.yellow;
            }
        }
    }

    private void WinHack()
    {
        Debug.Log("SYSTEM BREACH SUCCESSFUL!");
        if (currentLazer != null)
        {
            currentLazer.OnPuzzleSolved();
        }
        gameObject.SetActive(false);
    }

    private void ResetNodeColors()
    {
        foreach (Button node in puzzleNodesInOrder)
        {
            node.image.color = Color.white;
        }

        if (dynamicWinningOrder != null && dynamicWinningOrder.Length > 0)
        {
            dynamicWinningOrder[0].image.color = Color.green; // Start point
            dynamicWinningOrder[dynamicWinningOrder.Length - 1].image.color = Color.red; // End point
        }
    }
}