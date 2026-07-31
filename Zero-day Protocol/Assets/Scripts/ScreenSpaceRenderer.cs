using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ScreenSpaceLineRenderer : MonoBehaviour
{
    [Header("UI Reference")]
    public RectTransform waveBoxArea; // Drag your UI Panel / Image here

    [Header("Wave Parameters")]
    public float amplitude = 50f;     // Vertical height of wave inside the box
    public float frequency = 2f;      // Number of wave cycles
    public int pointsCount = 100;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        DrawWaveInUIBox();
    }

    void DrawWaveInUIBox()
    {
        if (waveBoxArea == null) return;

        lineRenderer.positionCount = pointsCount;

        // 1. Get the local width and height of the assigned UI Panel box
        Rect rect = waveBoxArea.rect;
        float width = rect.width;
        float height = rect.height;

        for (int i = 0; i < pointsCount; i++)
        {
            float t = (float)i / (pointsCount - 1); // 0.0 to 1.0 across the box

            // Calculate X from left (-width/2) to right (+width/2) inside UI local space
            float localX = rect.xMin + (t * width);

            // Calculate Y sine offset inside UI local space
            float sinVal = Mathf.Sin(t * frequency * Mathf.PI * 2f);
            float localY = rect.center.y + (sinVal * amplitude);

            // Create point in UI Local Space (Z = -1 brings it slightly forward)
            Vector3 localPoint = new Vector3(localX, localY, -1f);

            // Convert UI Local Point directly into World Position
            Vector3 worldPos = waveBoxArea.TransformPoint(localPoint);

            lineRenderer.SetPosition(i, worldPos);
        }
    }
}