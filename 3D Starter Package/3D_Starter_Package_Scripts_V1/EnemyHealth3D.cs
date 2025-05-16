// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Gives enemies functionality for health, taking damage, and dropping items.
    /// </summary>
    public class EnemyHealth3D : MonoBehaviour
    {
        [Tooltip("How many times the enemy can get hit before being destroyed.")]
        [SerializeField] private int health = 3;

        [Tooltip("Enter the name of the tag that should damage this enemy.")]
        [SerializeField] private string damageTagName = "PlayerAttack";

        [Tooltip("Optional: Sound effect when enemy is hit.")]
        [SerializeField] private AudioClip hitSound;

        [Tooltip("Optional: Sound effect when enemy is destroyed.")]
        [SerializeField] private AudioClip deathSound;

        [Tooltip("Delay (in seconds) before the enemy is destroyed after losing all its health. Increase this if the enemy is being destroyed before its death animation fully plays.")]
        [SerializeField] private float delayBeforeDying = 0f;

        [Tooltip("Optional: Drag in the enemy's animator for hit and death animations.")]
        [SerializeField] private Animator animator;

        [Tooltip("Optional: Drag in a slider UI element to use as a health bar.")]
        [SerializeField] private Slider healthBar;

        [Tooltip("Optional: GameObject to be spawned when the enemy dies.")]
        [SerializeField] private GameObject drop;

        [Tooltip("Optional: Position the drop will be spawned at. If left empty, drop will spawn at the center of this GameObject.")]
        [SerializeField] private Transform dropTransform;

        [Space(20)]
        [SerializeField] private UnityEvent onEnemyDamaged, onEnemyDeath;

        private int maxHealth;
        private bool isDying = false;

        private void Start()
        {
            maxHealth = health;

            if (healthBar != null)
            {
                healthBar.minValue = 0f;
                healthBar.maxValue = 1f;
                healthBar.value = 1f;
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (!string.IsNullOrEmpty(damageTagName) && collision.collider.CompareTag(damageTagName))
            {
                Hit();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(damageTagName) && other.CompareTag(damageTagName))
            {
                Hit();
            }
        }

        public void Hit()
        {
            if (isDying)
            {
                // Ignore the hit and return out of this method if the enemy is already dying
                return;
            }

            // Reduce health
            health--;

            if (healthBar != null)
            {
                healthBar.value = Mathf.Clamp01((float)health / maxHealth);
            }

            if (health <= 0)
            {
                // Play death sound if it has been assigned
                if (deathSound != null)
                {
                    AudioSource.PlayClipAtPoint(deathSound, transform.position);
                }

                // If the animator has been assigned, send it a trigger
                if (animator != null)
                {
                    animator.SetTrigger("Death");
                }

                if (delayBeforeDying == 0f)
                {
                    // If the death delay is 0 seconds, die immediately
                    Die();
                }
                else
                {
                    isDying = true; // Set isDying to true so the enemy can't die twice
                    Invoke(nameof(Die), delayBeforeDying); // This will call the Die method after delayBeforeDying seconds
                }
            }
            else
            {
                // Play hit sound if it has been assigned
                if (hitSound != null)
                {
                    AudioSource.PlayClipAtPoint(hitSound, transform.position);
                }

                // If the animator has been assigned, send it a trigger
                if (animator != null)
                {
                    animator.SetTrigger("Hit");
                }

                onEnemyDamaged.Invoke();
            }
        }

        private void Die()
        {
            // If the drop has been assigned, spawn it on death
            if (drop != null)
            {
                GameObject newDrop;

                if (dropTransform == null)
                {
                    // dropTransform is null, spawn drop at the center of the enemy
                    newDrop = Instantiate(drop, transform.position, Quaternion.identity);
                }
                else
                {
                    // dropTransform has been assigned, spawn drop there
                    newDrop = Instantiate(drop, dropTransform.position, Quaternion.identity);
                }

                // Make sure the drop is active when spawned
                newDrop.SetActive(true);
            }

            onEnemyDeath.Invoke();

            // Finally, destroy this entire GameObject
            Destroy(gameObject);
        }
    }
}