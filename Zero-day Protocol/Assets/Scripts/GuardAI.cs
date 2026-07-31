using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SimpleGuard : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotationSpeed = 10f; 
    public float wallCheckDist = 0.8f;
    public LayerMask wallLayer;

    [Header("Detection")]
    public Transform player;
    public Transform visionPivot; 
    public float viewDist = 5f;
    public float viewAngle = 90f; 
    public float stopChaseDist = 8f;

    private Rigidbody2D rb;
    private Vector2 spawnPos;
    private Vector2 patrolDir;
    private bool isChasing = false;
    private bool isReturning = false;
    private bool isSpawnPosInitialized = false; // Tracks procedural generation shift 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();


        rb.gravityScale = 0;

        // This keep the guard not "Spining"
        rb.freezeRotation = true;

        rb.linearDamping = 0;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        PickRandomAxis();

        // Find Player
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
            else
            {
                Debug.LogError("GUARD AI ERROR: Cannot find a GameObject with the tag 'Player' in the scene! Ensure your Player object is tagged 'Player'.");
            }
        }
    }

    void FixedUpdate()
    {
        // Only have the base position after level generation
        if (!isSpawnPosInitialized && transform.parent != null)
        {
            spawnPos = transform.position;
            isSpawnPosInitialized = true;
        }

        if (player == null) return;

        float distToPlayer = Vector2.Distance(rb.position, player.position);

        // 2. DETECTION LOGIC
        if (distToPlayer < viewDist && !isChasing)
        {
            if (IsInViewCone() && HasLineOfSight(distToPlayer))
            {
                isChasing = true;
                isReturning = false;
                Debug.Log($"[Guard AI] Player detected! Commencing active pursuit.");
            }
        }

        // 3. MOVEMENT LOGIC
        Vector2 desiredVelocity = Vector2.zero;
        if (isChasing) desiredVelocity = ChaseLogic(distToPlayer);
        else if (isReturning) desiredVelocity = ReturnLogic();
        else desiredVelocity = PatrolLogic();

        rb.linearVelocity = desiredVelocity;

        // 4. VISION ROTATION LOGIC
        if (desiredVelocity.sqrMagnitude > 0.01f && visionPivot != null)
        {
            float targetAngle = Mathf.Atan2(desiredVelocity.y, desiredVelocity.x) * Mathf.Rad2Deg - 90f;
            float angle = Mathf.LerpAngle(visionPivot.eulerAngles.z, targetAngle, rotationSpeed * Time.fixedDeltaTime);
            visionPivot.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    bool IsInViewCone()
    {
        if (visionPivot == null) return false;

        Vector2 dirToPlayer = ((Vector2)player.position - rb.position).normalized;

        // Check angle against the visionPivot's forward direction (up)
        float angleToPlayer = Vector2.Angle(visionPivot.up, dirToPlayer);

        return angleToPlayer < (viewAngle / 2f);
    }

    Vector2 PatrolLogic()
    {
        Vector2 rayStart = rb.position + (patrolDir * 0.6f);
        RaycastHit2D hit = Physics2D.Raycast(rayStart, patrolDir, wallCheckDist, wallLayer);

        if (hit.collider != null)
        {
            patrolDir = -patrolDir;
        }
        return patrolDir * moveSpeed;
    }

    Vector2 ChaseLogic(float dist)
    {
        if (dist > stopChaseDist)
        {
            isChasing = false;
            isReturning = true;
            Debug.Log($"[Guard AI] Target lost distance limit exceeded. Returning to patrol point: {spawnPos}");
            return Vector2.zero;
        }
        Vector2 dir = ((Vector2)player.position - rb.position).normalized;
        return dir * moveSpeed;
    }

    Vector2 ReturnLogic()
    {
        Vector2 dir = (spawnPos - rb.position).normalized;
        if (Vector2.Distance(rb.position, spawnPos) < 0.2f)
        {
            isReturning = false;
            PickRandomAxis();
            return Vector2.zero;
        }
        return dir * moveSpeed;
    }

    bool HasLineOfSight(float dist)
    {
        Vector2 dirToPlayer = ((Vector2)player.position - rb.position).normalized;
        Vector2 rayStart = rb.position + (dirToPlayer * 0.6f);

        // Raycast on wall
        RaycastHit2D hit = Physics2D.Raycast(rayStart, dirToPlayer, dist, wallLayer);

        return hit.collider == null; 
    }

    void PickRandomAxis()
    {
        patrolDir = (Random.value > 0.5f) ? Vector2.right : Vector2.up;
        if (Random.value > 0.5f) patrolDir *= -1;
    }

    private void OnDrawGizmosSelected()
    {
        if (visionPivot == null) return;

        Gizmos.color = Color.yellow;
        Vector3 leftLimit = Quaternion.AngleAxis(-viewAngle / 2f, Vector3.forward) * visionPivot.up;
        Vector3 rightLimit = Quaternion.AngleAxis(viewAngle / 2f, Vector3.forward) * visionPivot.up;

        Gizmos.DrawRay(transform.position, leftLimit * viewDist);
        Gizmos.DrawRay(transform.position, rightLimit * viewDist);
    }
}