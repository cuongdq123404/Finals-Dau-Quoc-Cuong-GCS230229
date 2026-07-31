using UnityEngine;

public class MagneticPylon : MonoBehaviour
{
    [Header("Repulsor Settings")]
    public float pushRadius = 5.0f;
    public float pushStrength = 8.0f;

    public bool isActive = true;

    private void FixedUpdate()
    {
        if (!isActive) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb == null) return;

        // Calculate distance and direction from pylon to player
        Vector2 directionToPlayer = (playerRb.position - (Vector2)transform.position);
        float distance = directionToPlayer.magnitude;

        // Check player is within the push radius
        if (distance <= pushRadius)
        {
            // Normalize the direction
            Vector2 normalizedDirection = directionToPlayer.normalized;
            Vector2 downDirection = -transform.up;

            // If the value is above 0, the player is on the "down" side
            // (Vector2.Dot returns > 0 if vectors point in roughly the same direction)
            if (Vector2.Dot(normalizedDirection, downDirection) > 0f)
            {
                // Apply force to push player
                playerRb.AddForce(normalizedDirection * pushStrength, ForceMode2D.Force);
            }
        }
    }

    // Draw the half-circle push field in the editor scene view
    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1f, 0.4f, 0f, 0.3f);
        Vector3 center = transform.position;
        Vector3 down = -transform.up;
        Vector3 right = transform.right;

        // Draw the outer arc boundary
        int segments = 20;
        Vector3 previousPoint = center + (Quaternion.AngleAxis(-90, transform.forward) * down * pushRadius);

        for (int i = -90; i <= 90; i += 180 / segments)
        {
            Vector3 nextPoint = center + (Quaternion.AngleAxis(i, transform.forward) * down * pushRadius);
            Gizmos.DrawLine(previousPoint, nextPoint);
            previousPoint = nextPoint;
        }

        // Draw the flat line
        Gizmos.DrawLine(center + right * pushRadius, center - right * pushRadius);
    }
}