// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
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

        [Tooltip("Time in seconds that the enemy will be invincible after taking damage.")]
        [SerializeField] private float invincibilityTime = 0.1f;

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

        public event System.Action<int, int> OnEnemyHealthChanged;

        private int maxHealth;
        private bool isDying = false;
        private bool isInvincible = false;
        private Coroutine invincibilityRoutine;

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
            if (collision.gameObject.TryGetComponent(out Damager damager) && damager.enabled)
            {
                Alignment alignment = damager.alignment;
                if (alignment == Alignment.Player || alignment == Alignment.Environment)
                {
                    if (damager.instakill)
                    {
                        TakeDamage(health, bypassInvincibility: true);
                        return;
                    }

                    if (damager.healInstead)
                    {
                        Heal(damager.damage);
                    }
                    else
                    {
                        TakeDamage(damager.damage);
                    }

                    return;
                }
            }

            if (!string.IsNullOrEmpty(damageTagName) && collision.collider.CompareTag(damageTagName))
            {
                TakeDamage();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.TryGetComponent(out Damager damager) && damager.enabled)
            {
                Alignment alignment = damager.alignment;
                if (alignment == Alignment.Player || alignment == Alignment.Environment)
                {
                    if (damager.instakill)
                    {
                        TakeDamage(health, bypassInvincibility: true);
                        return;
                    }

                    if (damager.healInstead)
                    {
                        Heal(damager.damage);
                    }
                    else
                    {
                        TakeDamage(damager.damage);
                    }

                    return;
                }
            }

            if (!string.IsNullOrEmpty(damageTagName) && collision.CompareTag(damageTagName))
            {
                TakeDamage();
            }
        }

        public void Heal(int amount)
        {
            if (amount > 0)
            {
                TakeDamage(-amount, bypassInvincibility: true);
            }
        }

        public void DealDamage(int amount)
        {
            TakeDamage(amount, true);
        }

        private void TakeDamage(int damage = 1, bool bypassInvincibility = false)
        {
            if (isDying || damage == 0)
            {
                // Ignore the hit and return out of this method if the enemy is already dying
                return;
            }

            // Ignore damage when invincible unless explicitly bypassed
            if (damage > 0 && isInvincible && !bypassInvincibility)
            {
                return;
            }

            int oldHealth = health;

            // Reduce health, but prevent going below 0
            health = Mathf.Max(0, health - damage);

            // Invoke health changed C# event
            OnEnemyHealthChanged?.Invoke(oldHealth, health);

            if (healthBar != null)
            {
                healthBar.value = Mathf.Clamp01((float)health / maxHealth);
            }

            if (health <= 0)
            {
                StopInvincibility();

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

                StartInvincibility();
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

        private void StartInvincibility()
        {
            if (invincibilityTime <= 0f)
            {
                return;
            }

            if (invincibilityRoutine != null)
            {
                StopCoroutine(invincibilityRoutine);
            }

            invincibilityRoutine = StartCoroutine(InvincibilityCoroutine());
        }

        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            float t = 0f;

            while (t < invincibilityTime)
            {
                t += Time.deltaTime;
                yield return null;
            }

            isInvincible = false;
            invincibilityRoutine = null;
        }

        private void StopInvincibility()
        {
            if (invincibilityRoutine != null)
            {
                StopCoroutine(invincibilityRoutine);
                invincibilityRoutine = null;
            }

            isInvincible = false;
        }

        private void OnValidate()
        {
            // Enforce minimum values
            invincibilityTime = Mathf.Max(0f, invincibilityTime);
            delayBeforeDying = Mathf.Max(0f, delayBeforeDying);
            health = Mathf.Max(1, health);
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