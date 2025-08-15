// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Gives enemies functionality for health, taking damage, and dropping items.
    /// </summary>
    public class EnemyHealth2D : MonoBehaviour
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
        [SerializeField] private AnimationParameters animationParameters;

        [Tooltip("Optional: Drag in a slider UI element to use as a health bar.")]
        [SerializeField] private Slider healthBar;

        [Tooltip("Optional: GameObject to be spawned when the enemy dies.")]
        [SerializeField] private GameObject drop;

        [Tooltip("Optional: Position the drop will be spawned at. If left empty, drop will spawn at the center of this GameObject.")]
        [SerializeField] private Transform dropTransform;

        [Space(20)]
        [SerializeField] private UnityEvent onEnemyDamaged, onEnemyDeath;

        public event System.Action<int> OnEnemyLostHealth;

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

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Damager damager))
            {
                Alignment alignment = damager.alignment;
                if (alignment == Alignment.Player || alignment == Alignment.Environment)
                {
                    if (damager.healInstead)
                    {
                        Heal(damager.damage);
                    }
                    else
                    {
                        Hit(damager.damage);
                    }

                    return;
                }
            }

            if (!string.IsNullOrEmpty(damageTagName) && collision.collider.CompareTag(damageTagName))
            {
                Hit();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Damager damager))
            {
                Alignment alignment = damager.alignment;
                if (alignment == Alignment.Player || alignment == Alignment.Environment)
                {
                    if (damager.healInstead)
                    {
                        Heal(damager.damage);
                    }
                    else
                    {
                        Hit(damager.damage);
                    }

                    return;
                }
            }

            if (!string.IsNullOrEmpty(damageTagName) && collision.CompareTag(damageTagName))
            {
                Hit();
            }
        }

        public void Hit(int damage = 1)
        {
            if (isDying)
            {
                // Ignore the hit and return out of this method if the enemy is already dying
                return;
            }

            // Reduce health, but prevent going below 0
            health = Mathf.Max(0, health - damage);

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
                    animator.SetTrigger(animationParameters.Death);
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
            else if (damage > 0)
            {
                // Play hit sound if it has been assigned
                if (hitSound != null)
                {
                    AudioSource.PlayClipAtPoint(hitSound, transform.position);
                }

                // If the animator has been assigned, send it a trigger
                if (animator != null)
                {
                    animator.SetTrigger(animationParameters.Hit);
                }

                // Invoke damaged UnityEvent
                onEnemyDamaged.Invoke();

                // Invoke lost health C# event
                OnEnemyLostHealth?.Invoke(health);
            }
        }

        public void Die()
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

                // Make sure the drop starts active
                newDrop.SetActive(true);
            }

            onEnemyDeath.Invoke();

            // Finally, destroy this entire GameObject
            Destroy(gameObject);
        }

        public void Heal(int amount)
        {
            if (amount > 0)
            {
                Hit(-amount);
            }
        }

        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Trigger parameter: " + nameof(Hit))]
            public string Hit = "Hit";

            [Tooltip("Trigger parameter: " + nameof(Death))]
            public string Death = "Death";
        }
    }
}