using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    private float currentSpeedMultiplier = 1.0f; // Multiplier for stasis

    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;

    [HideInInspector]
    public bool isFrozen = false; // Terminal Check

    [Header("Sprites")]
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite sideSprite;

    [Header("Melee Action")]
    public float meleeRange = 1.2f;
    public float meleeCooldown = 10f;
    public LayerMask guardLayer;
    public Slider cooldownBar;

    private Vector2 movement;
    private float nextMeleeTime = 0f;

    void Update()
    {
        if (isFrozen) // FrozenStatus
        {
            movement = Vector2.zero; 
            HandleCooldownUI();      
            return;                
        }

        //Gather Input
        movement.x = Input.GetAxisRaw("Horizontal");
        movement.y = Input.GetAxisRaw("Vertical");

        UpdateSpriteDirection();

        // Melee Cooldown
        if (Input.GetKeyDown(KeyCode.F) && Time.time >= nextMeleeTime)
        {
            PerformMelee();
        }

        HandleCooldownUI();
    }

    void HandleCooldownUI()
    {
        if (cooldownBar == null) return;
        if (Time.time < nextMeleeTime)
        {
            cooldownBar.gameObject.SetActive(true);
            float timeLeft = nextMeleeTime - Time.time;
            cooldownBar.value = Mathf.Clamp01(1 - (timeLeft / meleeCooldown));
        }
        else
        {
            cooldownBar.gameObject.SetActive(false);
        }
    }

    void PerformMelee()
    {
        Collider2D hitGuard = Physics2D.OverlapCircle(transform.position, meleeRange, guardLayer);
        if (hitGuard != null)
        {
            Destroy(hitGuard.gameObject);
            nextMeleeTime = Time.time + meleeCooldown;
        }
       // Stun Melee if Cardiac
        CardiacWarden warden = hitGuard.GetComponent<CardiacWarden>();
        if (warden != null)
        {
            warden.TriggerStun(6.0f); 
            nextMeleeTime = Time.time + meleeCooldown;
        }
    }

    void UpdateSpriteDirection()
    {
        if (movement.y > 0) spriteRenderer.sprite = upSprite;
        else if (movement.y < 0) spriteRenderer.sprite = downSprite;
        else if (movement.x != 0)
        {
            spriteRenderer.sprite = sideSprite;
            spriteRenderer.flipX = (movement.x < 0);
        }
    }

    void FixedUpdate()
    {
        // if frozen , stop movement 
        if (isFrozen)
        {
            if (rb != null) rb.linearVelocity = Vector2.zero;
            return;
        }

        // Movement update while in stasis
        if (rb != null)
        {
            float finalSpeed = moveSpeed * currentSpeedMultiplier;
            rb.MovePosition(rb.position + movement.normalized * finalSpeed * Time.fixedDeltaTime);
        }
    }

    // Called by StasisVent.cs to modify speed without breaking base values
    public void SetSpeedMultiplier(float multiplier)
    {
        currentSpeedMultiplier = multiplier;
    }
}