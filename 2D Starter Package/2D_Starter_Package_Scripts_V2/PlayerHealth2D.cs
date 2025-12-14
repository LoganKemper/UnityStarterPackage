// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Gives the player health, dying, and respawning functionality.
    /// </summary>
    public class PlayerHealth2D : MonoBehaviour, IDamageable
    {
        public enum HealthType : byte
        {
            HealthBar,
            Segmented
        }

        [System.Serializable]
        public class Events
        {
            [Space(20)]
            public UnityEvent onPlayerDamaged, onPlayerHealed, onPlayerDeath, onPlayerRespawn;
        }

        [Header("Health Settings")]
        [Tooltip("The player's maximum allowed health points.")]
        [SerializeField] private int maxHealth = 3;

        [Tooltip("The player's current health points. This doesn't necessarily have to be the same as maxHealth at the start of the game.")]
        [SerializeField] private int currentHealth = 3;

        [Tooltip("If true, the player's currentHealth will be allowed to exceed maxHealth.")]
        [SerializeField] private bool allowOverhealing = false;

        [Tooltip("Time in seconds that the player will be invincible after taking damage.")]
        [SerializeField] private float invincibilityTime = 0.1f;

        [Header("UI")]
        [Tooltip("Pick whether you want to use a continuous health bar image, or discrete health segments on the UI.")]
        [SerializeField] private HealthType healthType = HealthType.HealthBar;

        [Tooltip("Drag in an image if the health bar health type is selected.")]
        [SerializeField] private Image healthBar;

        [Tooltip("Add an image for each health segment if the segmented health type is selected. Make sure the number of items in the array matches the value of maxHealth.")]
        [SerializeField] private Image[] healthSegments;

        [Header("Respawning")]
        [Tooltip("Drag in a transform tagged \"Respawn\". If left null, this script will try to find a respawn tag in the scene.")]
        [SerializeField] private Transform respawnPoint;

        [Tooltip("Number of seconds after health reaches 0 before the player respawns.")]
        [SerializeField] private float delayBeforeRespawn = 0f;

        [Header("UnityEvents")]
        [SerializeField] private Events events;

        public Alignment Alignment { get; private set; } = Alignment.Player;

        private Coroutine invincibilityCoroutine;
        private bool isDying;
        private bool isInvincible;

        public void SetMaxHealth(int maxHealth)
        {
            this.maxHealth = maxHealth;
        }

        public void SetAllowOverhealing(bool allowOverhealing)
        {
            this.allowOverhealing = allowOverhealing;
        }

        public void SetInvincibilityTime(float invincibilityTime)
        {
            this.invincibilityTime = invincibilityTime;
        }

        public void SetDelayBeforeRespawn(float delayBeforeRespawn)
        {
            this.delayBeforeRespawn = delayBeforeRespawn;
        }

        private void Start()
        {
            if (healthType == HealthType.HealthBar)
            {
                if (healthBar != null)
                {
                    // The image type needs to be set to Filled to dynamically adjust its fill proportion
                    healthBar.type = Image.Type.Filled;
                }
            }
            else if (healthType == HealthType.Segmented)
            {
                if (maxHealth != healthSegments.Length)
                {
                    Debug.LogWarning("The player's maxHealth does not match the number of health segments!");
                }
            }

            UpdateHealthUI();

            // Try to find a respawn point if it hasn't been assigned
            if (respawnPoint == null)
            {
                GameObject respawn = GameObject.FindWithTag("Respawn");

                if (respawn == null)
                {
                    Debug.LogWarning("Player respawn point not found");
                }
                else
                {
                    respawnPoint = respawn.transform;
                }
            }
        }

        // Updates the health display on the UI
        private void UpdateHealthUI()
        {
            if (healthType == HealthType.HealthBar)
            {
                float fill = (float)currentHealth / maxHealth;

                if (fill > 1)
                {
                    fill = 1f;
                }

                if (healthBar != null)
                {
                    healthBar.fillAmount = fill;
                }
            }
            else if (healthType == HealthType.Segmented)
            {
                for (int i = 0; i < healthSegments.Length; i++)
                {
                    if (healthSegments[i] != null)
                    {
                        // Enables segments corresponding to the current health and disables extra segments if the health decreases
                        healthSegments[i].enabled = (i < currentHealth);
                    }
                }
            }
        }

        public bool CanBeDamagedBy(Alignment alignment)
        {
            return alignment switch
            {
                Alignment.Player => false,
                Alignment.Enemy => true,
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
                SetHealth(currentHealth + health);
            }
        }

        public void Instakill()
        {
            TakeDamage(int.MaxValue, bypassInvincibility: true);
        }

        public void SetHealth(int newHealth)
        {
            if (isDying)
            {
                return;
            }

            if (newHealth > currentHealth)
            {
                events.onPlayerHealed.Invoke();
            }
            else if (newHealth < currentHealth && newHealth > 0)
            {
                events.onPlayerDamaged.Invoke();
            }

            if (allowOverhealing)
            {
                // Allow overhealing, but don't allow negative values
                currentHealth = Mathf.Max(0, newHealth);
            }
            else
            {
                // Clamp health between 0 and maxHealth
                currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            }

            if (currentHealth == 0)
            {
                Die();
            }

            UpdateHealthUI();
        }

        public void SetHealthWithoutDying(int newHealth)
        {
            if (isDying)
            {
                return;
            }

            if (newHealth > currentHealth)
            {
                events.onPlayerHealed.Invoke();
            }
            else if (newHealth < currentHealth && newHealth > 0)
            {
                events.onPlayerDamaged.Invoke();
            }

            if (allowOverhealing)
            {
                currentHealth = Mathf.Max(0, newHealth);
            }
            else
            {
                currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
            }

            UpdateHealthUI();
        }

        private void Die()
        {
            events.onPlayerDeath.Invoke();

            StopInvincibility();

            if (delayBeforeRespawn > 0)
            {
                isDying = true;
                Invoke(nameof(Respawn), delayBeforeRespawn);
            }
            else
            {
                Respawn();
            }
        }

        private void Respawn()
        {
            isDying = false;

            // Move the player to the respawn point
            transform.position = respawnPoint.position;

            // Restore the player's health to the maximum
            int previousHealth = currentHealth;
            currentHealth = maxHealth;

            if (previousHealth != currentHealth)
            {
                UpdateHealthUI();
            }

            StopInvincibility();

            events.onPlayerRespawn.Invoke();
        }

        private void TakeDamage(int amount, bool bypassInvincibility = false)
        {
            if (amount <= 0 || isDying)
            {
                return;
            }

            // Ignore damage when invincible unless explicitly bypassed
            if (!bypassInvincibility && isInvincible)
            {
                return;
            }

            // Apply damage
            int newHealth = Mathf.Max(0, currentHealth - amount);
            SetHealth(newHealth);

            // If we died, no need to start invincibility frames
            if (currentHealth == 0)
            {
                return;
            }

            // Start invincibility only when the player actually took damage
            StartInvincibility();
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
            float t = 0f;

            while (t < invincibilityTime)
            {
                t += Time.deltaTime;
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
            // Make sure maxHealth can't be less than 1
            maxHealth = Mathf.Max(1, maxHealth);

            // Make sure delayBeforeRespawn can't be negative
            delayBeforeRespawn = Mathf.Max(0, delayBeforeRespawn);

            // Make sure invincibilityTime can't be negative
            invincibilityTime = Mathf.Max(0, invincibilityTime);

            // Make sure currentHealth can't be less than 0 or greater than maxHealth (if allowOverhealing is false)
            if (currentHealth < 0)
            {
                currentHealth = 0;
            }
            else if (currentHealth > maxHealth && !allowOverhealing)
            {
                currentHealth = maxHealth;
            }
        }
    }
}