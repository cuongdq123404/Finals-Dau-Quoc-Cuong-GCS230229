using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MissionMenuController : MonoBehaviour
{
    [Header("UI Buttons")]
    public Button stage2Button;
    public Button stage3Button;
    public Button bossButton;

    void OnEnable()
    {
        // Every time the menu opens, check progression status to unlock stages
        UpdateMenuProgress();
    }

    public void UpdateMenuProgress()
    {
        // Simple progression tracking check
        if (GameProgression.Instance != null)
        {
            // Stage 2 unlocks if we collected the item from Stage 1
            if (stage2Button != null)
                stage2Button.interactable = GameProgression.Instance.stage1ItemCollected;

            // Stage 3 unlocks if we collected the item from Stage 2
            if (stage3Button != null)
                stage3Button.interactable = GameProgression.Instance.stage2ItemCollected;

            // Boss fight unlocks only when all three core items are found
            if (bossButton != null)
                bossButton.interactable = GameProgression.Instance.CanUnlockBoss();
        }
    }

    // --- Button Click Functions ---

    public void LoadStage1()
    {
        Debug.Log("Loading Procedural Stage 1...");
        SceneManager.LoadScene(3);
    }

    public void LoadStage2()
    {
        Debug.Log("Loading Procedural Stage 2...");
        SceneManager.LoadScene(4);
    }

    public void LoadStage3()
    {
        Debug.Log("Loading Procedural Stage 3...");
        SceneManager.LoadScene(5);
    }

    public void LoadBossStage()
    {
        Debug.Log("Loading Final Boss Stage...");
        SceneManager.LoadScene(6);
    }
}