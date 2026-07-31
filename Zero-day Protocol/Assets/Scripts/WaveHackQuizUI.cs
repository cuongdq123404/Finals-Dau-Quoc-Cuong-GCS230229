using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WaveHackQuizUI : MonoBehaviour
{
    [Header("UI Panels & Text")]
    public GameObject quizCanvasPanel;
    public TextMeshProUGUI titleText;

    [Header("Wave Renderers")]
    public ScreenSpaceLineRenderer targetWave;
    public ScreenSpaceLineRenderer playerWave;

    [Header("Sliders")]
    public Slider amplitudeSlider;
    public Slider frequencySlider;

    private Action onSuccessCallback;
    private bool isQuizActive = false; // Prevents double-triggering success

    void Start()
    {
        if (quizCanvasPanel != null)
        {
            quizCanvasPanel.SetActive(false);
        }
    }

    public void OpenWaveHack(Action callback)
    {
        // 1. Assign new callback for whichever terminal opened this
        onSuccessCallback = callback;
        isQuizActive = true;

        // 2. Show UI
        if (quizCanvasPanel != null)
        {
            quizCanvasPanel.SetActive(true);
        }

        if (titleText != null)
        {
            titleText.gameObject.SetActive(true);
            titleText.text = "ALIGN SIGNAL FREQUENCY...";
        }

        // 3. Generate a NEW random target for this terminal so it's a fresh puzzle!
        if (targetWave != null)
        {
            targetWave.amplitude = UnityEngine.Random.Range(40f, 80f);
            targetWave.frequency = UnityEngine.Random.Range(1.5f, 4.0f);
        }

        // 4. Reset sliders to default starting position
        if (amplitudeSlider != null) amplitudeSlider.value = amplitudeSlider.minValue;
        if (frequencySlider != null) frequencySlider.value = frequencySlider.minValue;

        UpdatePlayerWave();
    }

    public void UpdatePlayerWave()
    {
        if (!isQuizActive || playerWave == null || amplitudeSlider == null || frequencySlider == null) return;

        playerWave.amplitude = amplitudeSlider.value;
        playerWave.frequency = frequencySlider.value;

        CheckWinCondition();
    }

    private void CheckWinCondition()
    {
        if (!isQuizActive || targetWave == null || playerWave == null) return;

        bool ampMatch = Mathf.Abs(playerWave.amplitude - targetWave.amplitude) < 4f;
        bool freqMatch = Mathf.Abs(playerWave.frequency - targetWave.frequency) < 0.25f;

        if (ampMatch && freqMatch)
        {
            Debug.Log("[HACK SUCCESS] Signal Aligned!");
            CompleteHackSuccess();
        }
    }

    public void CompleteHackSuccess()
    {
        if (!isQuizActive) return;

        isQuizActive = false;

        if (quizCanvasPanel != null)
        {
            quizCanvasPanel.SetActive(false);
        }

        // Execute the terminal's callback and clear it
        Action tempCallback = onSuccessCallback;
        onSuccessCallback = null;
        tempCallback?.Invoke();
    }
}