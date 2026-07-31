using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Targeting")]
    public string playerTag = "Player";
    public float detectionRange = 7.0f;
    public Transform rotationPivot;    // The rotating head/barrel of the turret

    [Header("Sprite Alignment")]
    [Tooltip("Adjust this offset if your sprite points the wrong way by default (typically 0, 90, or -90)")]
    public float angleOffset = -90f;   // Set to -90 if your sprite's gun nozzle points straight down by default

    [Header("Shooting Settings")]
    public GameObject projectilePrefab;
    public Transform muzzlePoint;
    public float fireRate = 1.5f;

    private Transform targetPlayer;
    private float fireTimer;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag(playerTag);
        if (playerObj != null)
        {
            targetPlayer = playerObj.transform;
        }
    }

    void Update()
    {
        if (targetPlayer == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, targetPlayer.position);

        if (distanceToPlayer <= detectionRange)
        {
            TrackPlayer360();
            HandleShooting();
        }
    }

    void TrackPlayer360()
    {
        if (rotationPivot == null) return;

        // 1. Get the vector pointing from the pivot to the player
        Vector3 direction = targetPlayer.position - rotationPivot.position;

        // 2. Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // 3. Rotate the pivot freely on the Z-axis, incorporating your sprite's default offset
        rotationPivot.rotation = Quaternion.Euler(0, 0, angle + angleOffset);
    }

    void HandleShooting()
    {
        fireTimer += Time.deltaTime;

        if (fireTimer >= fireRate)
        {
            fireTimer = 0f;
            Shoot();
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || muzzlePoint == null || targetPlayer == null) return;

        // 1. Calculate the exact direction from the turret center toward the player
        Vector2 shootDirection = (targetPlayer.position - transform.position).normalized;

        // 2. Determine the spawn position dynamically
        // Instead of trusting the static muzzle point, we place it a set distance (e.g., 0.6 units)
        // away from the turret center, pushing it straight toward the target player.
        float barrelLength = 0.6f;
        Vector3 dynamicSpawnPosition = transform.position + (Vector3)(shootDirection * barrelLength);

        // 3. Calculate the bullet's rotation angle (facing the target)
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        // Set to (angle - 90f) if your bullet script uses transform.up to fly forward
        Quaternion bulletRotation = Quaternion.Euler(0, 0, angle - 90f);

        // 4. Spawn the bullet at the dynamically calculated outer edge
        Instantiate(projectilePrefab, dynamicSpawnPosition, bulletRotation);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}