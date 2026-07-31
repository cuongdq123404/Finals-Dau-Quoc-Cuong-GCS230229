using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement; // Needed to restart the level!

public class CardiacWarden : MonoBehaviour
{
    public enum WardenState { Patrolling, OverdriveChase, Stunned }

    [Header("State Settings")]
    public WardenState currentState = WardenState.Patrolling;

    [Header("Movement Speeds")]
    public float patrolSpeed = 2f;
    public float overdriveSpeed = 5.5f;

    [Header("Targeting & Vision")]
    public Transform player;
    public float visionRange = 6f;
    public LayerMask obstacleLayer;

    [Header("Patrol Path")]
    public Transform[] patrolWaypoints;
    private int currentWaypointIndex = 0;

    [Header("State Sprites (No Animation Needed!)")]
    public Sprite patrolSprite;
    public Sprite overdriveSprite;
    public Sprite stunnedSprite;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (player == null)
        {
            GameObject p = GameObject.FindWithTag("Player");
            if (p != null) player = p.transform;
        }

        UpdateSpriteState();
    }

    void Update()
    {
        switch (currentState)
        {
            case WardenState.Patrolling:
                PatrolBehavior();
                CheckForPlayer();
                break;

            case WardenState.OverdriveChase:
                ChaseBehavior();
                break;

            case WardenState.Stunned:
                rb.linearVelocity = Vector2.zero;
                break;
        }
    }

    void PatrolBehavior()
    {
        if (patrolWaypoints.Length == 0) return;

        Transform targetWaypoint = patrolWaypoints[currentWaypointIndex];
        moveDirection = (targetWaypoint.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * patrolSpeed;

        if (Vector2.Distance(transform.position, targetWaypoint.position) < 0.2f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolWaypoints.Length;
        }
    }

    void CheckForPlayer()
    {
        if (player == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, player.position);
        if (distanceToPlayer <= visionRange)
        {
            Vector2 directionToPlayer = (player.position - transform.position).normalized;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directionToPlayer, visionRange, obstacleLayer);

            if (hit.collider == null || hit.collider.CompareTag("Player"))
            {
                TriggerOverdrive();
            }
        }
    }

    void ChaseBehavior()
    {
        if (player == null) return;

        moveDirection = (player.position - transform.position).normalized;
        rb.linearVelocity = moveDirection * overdriveSpeed;
    }

    public void TriggerOverdrive()
    {
        if (currentState == WardenState.OverdriveChase) return;

        currentState = WardenState.OverdriveChase;
        UpdateSpriteState();
    }

    public void TriggerStun(float duration)
    {
        StartCoroutine(StunRoutine(duration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        WardenState previousState = currentState;
        currentState = WardenState.Stunned;
        UpdateSpriteState();

        yield return new WaitForSeconds(duration);

        currentState = previousState;
        UpdateSpriteState();
    }

    void UpdateSpriteState()
    {
        if (spriteRenderer == null) return;

        switch (currentState)
        {
            case WardenState.Patrolling:
                if (patrolSprite != null) spriteRenderer.sprite = patrolSprite;
                break;
            case WardenState.OverdriveChase:
                if (overdriveSprite != null) spriteRenderer.sprite = overdriveSprite;
                break;
            case WardenState.Stunned:
                if (stunnedSprite != null) spriteRenderer.sprite = stunnedSprite;
                break;
        }
    }

    // --- NEW: KILL PLAYER ON TOUCH ---
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks if the object touched has the "Player" tag
        if (collision.gameObject.CompareTag("Player"))
        {
            RestartLevel();
        }
    }

    // Fallback if your player or guard uses triggers instead of solid physics colliders
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RestartLevel();
        }
    }

    void RestartLevel()
    {
        Debug.Log("Warden caught the player! Restarting stage...");
        // Gets the current active scene and reloads it cleanly
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    // Inside CardiacWardenNoAnim.cs

    [Header("Stagger Settings")]
    public float bulletStaggerDuration = 1.5f; // Short freeze when shot

    public void OnHitByBullet()
    {
        // Don't override if already heavily stunned by a vent
        if (currentState == WardenState.Stunned) return;

        Debug.Log("[WARDEN] Shot by player! Staggered!");
        TriggerStun(bulletStaggerDuration);
    }
}