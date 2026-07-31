using UnityEngine;
using UnityEngine.SceneManagement; // Required to reload/restart the level

public class TurretProjectile : MonoBehaviour
{
    public float speed = 15f;
    public float lifeTime = 2f;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Ensure the bullet doesn't rotate on its own once fired
        rb.freezeRotation = true;

        // Use transform.up because the turret rotates the bullet to face its target
        rb.linearVelocity = transform.up * speed;

        Destroy(gameObject, lifeTime); // Failsafe cleanup
    }

    // This runs the moment the solid shapes touch
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 1. If the bullet touches the Player, restart the level
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Player hit by turret! Restarting level...");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            Destroy(gameObject);
            return;
        }

        // 2. If the bullet touches a Guard, phase through them completely
        if (collision.gameObject.CompareTag("Guard"))
        {
            // Tell the physics engine to ignore this collision so they slide past each other
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            return;
        }

        // 3. If it hits an obstacle layer or wall, destroy the bullet
        if (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle"))
        {
            Debug.Log("Turret bullet hit an obstacle: " + collision.gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            // Failsafe: Destroy if hitting other environment walls/boundaries
            Destroy(gameObject);
        }
    }
}