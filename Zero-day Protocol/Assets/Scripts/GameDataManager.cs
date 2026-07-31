using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    [Header("Campaign Progression")]
    // index 0 = Stage 1, index 1 = Stage 2, index 2 = Stage 3...
    [Tooltip("Check these boxes to see which stages the player has completed (earned a trophy).")]
    public bool[] stageTrophies = new bool[3];

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Call this when a stage is cleared with the key item.
    /// </summary>
    /// <param name="stageNumber">The stage number that was cleared (e.g., 1, 2, or 3)</param>
    public void AwardStageClear(int stageNumber)
    {
        // Arrays start counting at 0, so Stage 1 maps to index 0, Stage 2 to index 1, etc.
        int arrayIndex = stageNumber - 1;

        if (arrayIndex >= 0 && arrayIndex < stageTrophies.Length)
        {
            stageTrophies[arrayIndex] = true;
            Debug.Log($"[DATA MANAGER] Stage {stageNumber} Completed! Trophy saved.");
        }
        else
        {
            Debug.LogError($"[DATA MANAGER] Attempted to clear Stage {stageNumber}, but it is out of bounds for our array!");
        }
    }

    /// <summary>
    /// Checks if a specific stage has been unlocked yet.
    /// </summary>
    public bool IsStageUnlocked(int stageNumber)
    {
        // Stage 1 is always unlocked by default
        if (stageNumber == 1) return true;

        // For higher stages, check if the previous stage's trophy was earned
        int previousStageIndex = stageNumber - 2;

        if (previousStageIndex >= 0 && previousStageIndex < stageTrophies.Length)
        {
            return stageTrophies[previousStageIndex];
        }

        return false;
    }
}