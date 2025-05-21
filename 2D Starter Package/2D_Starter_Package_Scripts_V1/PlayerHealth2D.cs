// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Gives the player health, dying, and respawning functionality.
    /// </summary>
    public class PlayerHealth2D : MonoBehaviour
    {
        [Tooltip("The player's maximum allowed health points.")]
        [SerializeField] private int maxHealth = 3;

        [Tooltip("The player's current health points. This doesn't necessarily have to be the same as maxHealth at the start of the game.")]
        [SerializeField] private int currentHealth = 3;

        [Tooltip("If true, the player's currentHealth will be allowed to exceed maxHealth.")]
        [SerializeField] private bool allowOverhealing = false;

        [Tooltip("Pick whether you want to use a continuous health bar image, or discrete health segments on the UI.")]
        [SerializeField] private HealthType healthType = HealthType.HealthBar;

        [Tooltip("Drag in an image if the health bar health type is selected.")]
        [SerializeField] private Image healthBar;

        [Tooltip("Add an image for each health segment if the segmented health type is selected. Make sure the number of items in the array matches the value of maxHealth.")]
        [SerializeField] private Image[] healthSegments;

        [Tooltip("Drag in a transform tagged \"Respawn\". If left null, this script will try to find a respawn tag in the scene.")]
        [SerializeField] private Transform respawnPoint;

        [Tooltip("Number of seconds after health reaches 0 before the player respawns.")]
        [SerializeField] private float delayBeforeRespawn = 0f;

        [Tooltip("Optional: Sound effect for when the player has been damaged.")]
        [SerializeField] private AudioClip damagedSound;

        [Tooltip("Optional: Sound effect for when the player has died.")]
        [SerializeField] private AudioClip deathSound;

        [Space(20)]
        [SerializeField] private UnityEvent onPlayerDamaged, onPlayerHealed, onPlayerDeath, onPlayerRespawn;

        private bool isDying = false;

        public enum HealthType
        {
            HealthBar,
            Segmented
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

            UpdateHealth();

            // Try to find a respawn point if it hasn't been assigned
            if (respawnPoint == null)
            {
                GameObject respawn = GameObject.FindWithTag("Respawn");

                if (respawnPoint == null)
                {
                    Debug.LogWarning("Player repawn point not found");
                }
                else
                {
                    respawnPoint = respawn.transform;
                }
            }
        }

        // Updates the health display on the UI
        private void UpdateHealth()
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

        private void SetHealth(int newHealth)
        {
            if (isDying)
            {
                return;
            }

            if (newHealth > currentHealth)
            {
                onPlayerHealed.Invoke();
            }
            else if (newHealth < currentHealth && newHealth > 0)
            {
                onPlayerDamaged.Invoke();
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

            UpdateHealth();
        }

        private void Die()
        {
            onPlayerDeath.Invoke();

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
            SetHealth(maxHealth);

            onPlayerRespawn.Invoke();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Enemy"))
            {
                SetHealth(currentHealth - 1);
            }
            else if (collision.CompareTag("Death"))
            {
                SetHealth(0);
            }
            else if (collision.CompareTag("Health"))
            {
                SetHealth(currentHealth + 1);
            }
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Enemy"))
            {
                SetHealth(currentHealth - 1);
            }
            else if (collision.collider.CompareTag("Death"))
            {
                SetHealth(0);
            }
            else if (collision.collider.CompareTag("Health"))
            {
                SetHealth(currentHealth + 1);
            }
        }

        private void OnValidate()
        {
            // Make sure the maxHealth can't be less than 1
            if (maxHealth < 1)
            {
                maxHealth = 1;
            }

            // Make sure currentHealth can't be less than 1 or greater than maxHealth (if allowOverhealing is false)
            if (currentHealth < 1)
            {
                currentHealth = 1;
            }
            else if (currentHealth > maxHealth && !allowOverhealing)
            {
                currentHealth = maxHealth;
            }

            // Make sure delayBeforeRespawn can't be negative
            if (delayBeforeRespawn < 0)
            {
                delayBeforeRespawn = 0;
            }
        }
    }
}