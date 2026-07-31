using System.Collections;
using UnityEngine;

public class StasisVent : MonoBehaviour
{
    [Header("Timing Settings")]
    public float activeDuration = 3.0f;   
    public float inactiveDuration = 3.0f; 

    [Header("Stasis Mechanics")]
    [Range(0.1f, 0.9f)]
    public float speedMultiplier = 0.4f;  // 40% speed 
    public float wardenStunDuration = 4.0f; 

    private bool fieldIsActive = true;
    private PlayerController playerInVent = null;

    private void Start()
    {
        StartCoroutine(VentCycleLoop());
    }

    private IEnumerator VentCycleLoop()
    {
        while (true)
        {
            // 1. Turn ON the slowing field
            fieldIsActive = true;
            Debug.Log("[VENT] Stasis Field ACTIVE.");

            ApplySlowEffect();

            yield return new WaitForSeconds(activeDuration);

            // 2. Turn OFF the slowing field
            fieldIsActive = false;
            Debug.Log("[VENT] Stasis Field INACTIVE.");
            RemoveSlowEffect();

            yield return new WaitForSeconds(inactiveDuration);
        }
    }

    private void ApplySlowEffect()
    {
        if (fieldIsActive && playerInVent != null)
        {
            // Set the slow multiplier on the player safely
            playerInVent.SetSpeedMultiplier(speedMultiplier);
            Debug.Log("[VENT] Player slowed by stasis frost!");
        }
    }

    private void RemoveSlowEffect()
    {
        if (playerInVent != null)
        {
            // Restore normal speed (100% / multiplier = 1.0f)
            playerInVent.SetSpeedMultiplier(1.0f);
            Debug.Log("[VENT] Player speed restored.");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // --- 1. HANDLE PLAYER ---
        if (other.CompareTag("Player"))
        {
            playerInVent = other.GetComponent<PlayerController>();
            ApplySlowEffect();
        }

        // --- 2. HANDLE WARDEN GUARD ---
        if (other.CompareTag("Guard"))
        {
            if (fieldIsActive)
            {
                CardiacWarden warden = other.GetComponent<CardiacWarden>();
                if (warden != null)
                {
                    warden.TriggerStun(wardenStunDuration);
                    Debug.Log("[VENT] Warden caught in stasis vent! Stunned!");
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            RemoveSlowEffect();
            playerInVent = null;
        }
    }

    // Call this from a hacking terminal if you want to force the vent active!
    public void ForceActivateVent()
    {
        StopAllCoroutines();
        StartCoroutine(VentCycleLoop());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = fieldIsActive ? new Color(0f, 0.8f, 1f, 0.3f) : new Color(0.5f, 0.5f, 0.5f, 0.1f);
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawCube(col.bounds.center, col.bounds.size);
        }
    }
}