// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Gives enemies functionality for health, taking damage, and dropping items.
    /// </summary>
    public class EnemyHealth3D : MonoBehaviour, IDamageable
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Trigger parameter.")]
            public string hitTrigger = "Hit";

            [Tooltip("Trigger parameter")]
            public string deathTrigger = "Death";
        }

        [System.Serializable]
        public class Events
        {
            [Space(20)]
            public UnityEvent onEnemyDamaged, onEnemyDeath;
        }

        [Tooltip("The enemy's maximum allowed health points.")]
        [SerializeField] private int maxHealth = 3;

        [Tooltip("The enemy's current health points.")]
        [SerializeField] private int currentHealth = 3;

        [Tooltip("Time in seconds that the enemy will be invincible after taking damage.")]
        [SerializeField] private float invincibilityTime = 0.1f;

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

        [SerializeField] private Events events;

        public Alignment Alignment { get; private set; } = Alignment.Enemy;
        public event System.Action<int, int> OnEnemyHealthChanged;

        private Coroutine invincibilityCoroutine;
        private bool isDying;
        private bool isInvincible;

        public void SetMaxHealth(int maxHealth)
        {
            this.maxHealth = maxHealth;

            if (currentHealth > maxHealth)
            {
                SetHealth(maxHealth);
                return;
            }

            UpdateHealthBar();
        }

        public void SetInvincibilityTime(float invincibilityTime)
        {
            this.invincibilityTime = invincibilityTime;
        }

        public void SetDelayBeforeDying(float delayBeforeDying)
        {
            this.delayBeforeDying = delayBeforeDying;
        }

        private void Start()
        {
            if (healthBar != null)
            {
                healthBar.wholeNumbers = false;
                healthBar.minValue = 0f;
                healthBar.maxValue = 1f;
                UpdateHealthBar();
            }
        }

        public bool CanBeDamagedBy(Alignment alignment)
        {
            return alignment switch
            {
                Alignment.Player => true,
                Alignment.Enemy => false,
                Alignment.Environment => true,
                _ => true,
            };
        }

        public void DealDamage(int damage)
        {
            TakeDamage(damage, bypassInvincibility: false);
        }

        public void Heal(int health)
        {
            if (health > 0)
            {
                TakeDamage(-health, bypassInvincibility: true);
            }
        }

        public void Instakill()
        {
            TakeDamage(int.MaxValue, true);
        }

        public void SetHealth(int health)
        {
            if (health == currentHealth)
            {
                return;
            }

            int oldHealth = currentHealth;
            currentHealth = Mathf.Clamp(health, 0, maxHealth);
            OnEnemyHealthChanged?.Invoke(oldHealth, currentHealth);
            UpdateHealthBar();
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

            int oldHealth = currentHealth;

            // Adjust health, but prevent going below 0 or above the max
            currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);

            // Invoke health changed C# event
            OnEnemyHealthChanged?.Invoke(oldHealth, currentHealth);

            UpdateHealthBar();

            if (currentHealth <= 0)
            {
                StopInvincibility();

                // If the animator has been assigned, send it a trigger
                if (animator != null)
                {
                    animator.SetTrigger(animationParameters.deathTrigger);
                }

                isDying = true; // Set isDying to true so the enemy can't die twice

                if (delayBeforeDying == 0f)
                {
                    // If the death delay is 0 seconds, die immediately
                    Die();
                }
                else
                {
                    // This will call the Die method after delayBeforeDying seconds
                    Invoke(nameof(Die), delayBeforeDying);
                }
            }
            else if (damage > 0)
            {
                // If the animator has been assigned, send it a trigger
                if (animator != null)
                {
                    animator.SetTrigger(animationParameters.hitTrigger);
                }

                // Invoke damaged UnityEvent
                events.onEnemyDamaged.Invoke();

                StartInvincibility();
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

                // Make sure the drop starts active
                newDrop.SetActive(true);
            }

            events.onEnemyDeath.Invoke();

            // Finally, destroy this entire GameObject
            Destroy(gameObject);
        }

        private void UpdateHealthBar()
        {
            if (healthBar != null)
            {
                healthBar.value = Mathf.Clamp01((float)currentHealth / maxHealth);
            }
        }

        private void StartInvincibility()
        {
            if (invincibilityTime <= 0f)
            {
                return;
            }

            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
            }

            invincibilityCoroutine = StartCoroutine(InvincibilityCoroutine());
        }

        private IEnumerator InvincibilityCoroutine()
        {
            isInvincible = true;
            float time = 0f;

            while (time < invincibilityTime)
            {
                time += Time.deltaTime;
                yield return null;
            }

            isInvincible = false;
            invincibilityCoroutine = null;
        }

        private void StopInvincibility()
        {
            if (invincibilityCoroutine != null)
            {
                StopCoroutine(invincibilityCoroutine);
                invincibilityCoroutine = null;
            }

            isInvincible = false;
        }

        private void OnValidate()
        {
            // Enforce minimum values
            invincibilityTime = Mathf.Max(0f, invincibilityTime);
            delayBeforeDying = Mathf.Max(0f, delayBeforeDying);
            maxHealth = Mathf.Max(1, maxHealth);

            // Make sure currentHealth can't be less than 0 or greater than maxHealth
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            else if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
    }
}