using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression Instance;

    [Header("Progression Items")]
    public bool stage1ItemCollected = false;
    public bool stage2ItemCollected = false;
    public bool stage3ItemCollected = false;

    void Awake()
    {
        // Keeps this progression data alive when switching scenes
        if (Instance == null)
        {
            Instance = this;
            transform.SetParent(null); // Detach from any parent to ensure it persists across scenes
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool CanUnlockBoss()
    {
        return stage1ItemCollected && stage2ItemCollected && stage3ItemCollected;
    }

}