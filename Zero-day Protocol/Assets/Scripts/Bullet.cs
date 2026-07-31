using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    public float wardenStaggerDuration = 10f; // How long the Warden gets stunned
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the bullet doesn't rotate on its own once fired
        rb.freezeRotation = true;

        // Change to transform.right if your bullet sprite points right by default
        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime); // Failsafe cleanup
    }

    // This runs the moment solid shapes touch
    void OnCollisionEnter2D(Collision2D collision)
    {
        // SAFETY FIX: If the bullet touches the Player, ignore it and keep flying
        if (collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        // 1. Check if we hit a Guard
        if (collision.gameObject.CompareTag("Guard"))
        {
            Debug.Log("Confirmed Hit on: " + collision.gameObject.name);

            // CHECK: Is this special Guard the Warden?
            CardiacWarden warden = collision.gameObject.GetComponentInParent<CardiacWarden>();

            if (warden != null)
            {
                // WARDEN HIT: Don't destroy him, just trigger his stagger/stun!
                warden.TriggerStun(wardenStaggerDuration);
                Debug.Log("[BULLET] Warden hit! Staggered for " + wardenStaggerDuration + "s.");
            }
            else
            {
                // NORMAL GUARD HIT: Destroy the regular guard immediately!
                Destroy(collision.gameObject);
                Debug.Log("[BULLET] Normal Guard killed!");
            }

            // Destroy the bullet on hit regardless
            Destroy(gameObject);
        }
        else
        {
            // 2. If it hits a wall or obstacle, destroy the bullet
            Debug.Log("Bullet hit a wall/object: " + collision.gameObject.name);
            Destroy(gameObject);
        }
    }
}