// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Gives the player a melee attack.
    /// </summary>
    public class PlayerMeleeAttack2D : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Trigger parameter: " + nameof(Melee))]
            public string Melee = "Melee";
        }

        [Header("Attack Settings")]
        [Tooltip("The button input used for the melee attack. Set to right click (Mouse1) by default.")]
        [SerializeField] private KeyCode buttonInput = KeyCode.Mouse1;

        [Tooltip("Drag in the hitbox GameObject with a trigger collider on it here.")]
        [SerializeField] private Collider2D hitbox;

        [Tooltip("How long the hitbox activates for.")]
        [SerializeField] private float hitboxTime = 0.1f;

        [Tooltip("How long after an attack can another attack be executed.")]
        [SerializeField] private float cooldown = 0.25f;

        [Header("Animation")]
        [Tooltip("Optional: Drag the player's animator in here for a melee animation trigger.")]
        [SerializeField] private Animator animator;

        [SerializeField] private AnimationParameters animationParameters;

        [Space(20)]
        [SerializeField] private UnityEvent onMeleeStart, onMeleeEnd;

        private Coroutine meleeCoroutine;
        private bool canMelee = true;
        private float cooldownTimer = 0;

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
            if (Input.GetKeyDown(buttonInput) && canMelee && cooldownTimer <= 0.01f)
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
            // If the animator has been assigned, send it a trigger
            if (animator != null)
            {
                animator.SetTrigger(animationParameters.Melee);
            }

            // Begin the attack
            cooldownTimer = cooldown;
            hitbox.enabled = true;
            onMeleeStart.Invoke();

            // Wait for hitboxTime, then disable the hitbox
            yield return new WaitForSeconds(hitboxTime);
            hitbox.enabled = false;
            onMeleeEnd.Invoke();
            meleeCoroutine = null;
        }
    }
}