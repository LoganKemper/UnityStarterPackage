// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gives the player a melee attack.
/// </summary>
public class PlayerMeleeAttack2D : MonoBehaviour
{
    [Tooltip("Drag in the hitbox GameObject with a trigger collider on it here.")]
    [SerializeField] private Collider2D hitbox;

    [Tooltip("Optional: Drag the player's animator in here for a melee animation.")]
    [SerializeField] private Animator animator;

    [Tooltip("Optional: Sound effect when melee activates.")]
    [SerializeField] private AudioClip meleeSound;

    [Tooltip("How long the hitbox activates for.")]
    [SerializeField] private float hitboxTime = 0.1f;

    [Tooltip("How long after an attack can another attack be executed.")]
    [SerializeField] private float cooldown = 0.25f;

    [Space(20)]
    [SerializeField] private UnityEvent onMelee;

    private bool canMelee = true;
    private float cooldownTimer = 0;
    private Coroutine meleeCoroutine;

    // Call from a UnityEvent to enable or disable the attack
    public void EnableMeleeAttack(bool enableAttack)
    {
        canMelee = enableAttack;
    }

    private void Start()
    {
        // Make sure the hitbox is disabled on start
        hitbox.enabled = false;
    }

    private void Update()
    {
        // Fire2 is the right mouse button by default
        if (Input.GetButtonDown("Fire2") && canMelee && cooldownTimer <= 0.01f)
        {
            if (meleeCoroutine != null)
            {
                StopCoroutine(meleeCoroutine);
            }

            meleeCoroutine = StartCoroutine(MeleeCoroutine());
        }

        // Subtract the time since the last frame from the cooldown timer
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    private IEnumerator MeleeCoroutine()
    {
        // If the animator has been assigned, send it a "Melee" trigger
        if (animator != null)
        {
            animator.SetTrigger("Melee");
        }

        // Play the melee sound if it has been assigned
        if (meleeSound != null)
        {
            AudioSource.PlayClipAtPoint(meleeSound, transform.position);
        }

        // Begin the attack
        cooldownTimer = cooldown;
        hitbox.enabled = true;
        onMelee.Invoke();

        // Wait for hitboxTime, then disable the hitbox
        yield return new WaitForSeconds(hitboxTime);
        hitbox.enabled = false;
    }
}
