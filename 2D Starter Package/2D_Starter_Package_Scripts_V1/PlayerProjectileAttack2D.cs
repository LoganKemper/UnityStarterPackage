// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Gives the player a projectile attack.
    /// </summary>
    public class PlayerProjectileAttack2D : MonoBehaviour
    {
        public enum LaunchDirection : byte
        {
            FacingDirection,
            MousePosition
        }

        [Header("Attack Settings")]
        [Tooltip("The button input used for the projectile attack. Set to left click (Mouse0) by default.")]
        [SerializeField] private KeyCode buttonInput = KeyCode.Mouse0;

        [Tooltip("Drag the projectile prefab in here. The projectile GameObject must have the Projectile2D component on it.")]
        [SerializeField] private Projectile2D projectile;

        [Tooltip("The position that the projectile should spawn from. It's usually a good idea to place it a little in front of the player.")]
        [SerializeField] private Transform launchTransform;

        [Tooltip("Optional: Assign the player GameObject to prevent projectiles from destroying when touching the player.")]
        [SerializeField] private GameObject player;

        [Tooltip("The initial velocity of the projectile.")]
        [SerializeField] private float velocity = 20f;

        [Tooltip("Cooldown time between projectiles.")]
        [SerializeField] private float cooldown = 0.05f;

        [Tooltip("Optional: Delay the spawning of the projectile. Leave at 0 to shoot immediately.")]
        [SerializeField] private float shootDelay = 0f;

        [Tooltip("If set to MousePosition, the projectile will shoot towards the mouse cursor. Otherwise the projectile will shoot in the direction the player is facing.")]
        [SerializeField] private LaunchDirection launchDirection = LaunchDirection.FacingDirection;

        [Header("Ammo Settings")]
        [Tooltip("Whether the projectile attack requires ammunition to work.")]
        [SerializeField] private bool requireAmmo = false;

        [Tooltip("The current quantitiy of ammunition.")]
        [SerializeField] private int ammo = 0;

        [Tooltip("How much ammo is consumed each shot.")]
        [SerializeField] private int ammoCost = 1;

        [Space(20)]
        [SerializeField] private UnityEvent onProjectileLaunched, onNoAmmo;

        [Space(20)]
        [SerializeField] private UnityEvent<int> onAmmoChanged;

        private bool canShoot = true;
        private bool isOnCooldown = false;
        private bool isWaitingDelayedShot = false;

        // Call this from a UnityEvent to enable/disable shooting
        public void EnableProjectileAttack(bool canShoot)
        {
            this.canShoot = canShoot;
        }

        // Call this from a UnityEvent to enable/disable ammo being required to shoot
        public void SetRequireAmmo(bool requireAmmo)
        {
            this.requireAmmo = requireAmmo;
        }

        // Call this from a UnityEvent to set the ammo count to a particular value
        public void SetAmmoCount(int count)
        {
            if (count >= 0)
            {
                ammo = count;
            }

            onAmmoChanged.Invoke(ammo);
        }

        // Call this from a UnityEvent to add/subtract ammo
        public void AdjustAmmoCount(int adjustment)
        {
            ammo += adjustment;

            if (ammo < 0)
            {
                ammo = 0;
            }

            onAmmoChanged.Invoke(ammo);
        }

        // Call this from a UnityEvent to set the ammo cost
        public void SetAmmoCost(int ammoCost)
        {
            this.ammoCost = ammoCost;
        }

        // Call this from a UnityEvent to set the cooldown time
        public void SetCooldown(float cooldown)
        {
            this.cooldown = cooldown;
        }

        private void Start()
        {
            if (requireAmmo)
            {
                onAmmoChanged.Invoke(ammo);
            }

            if (launchTransform == null)
            {
                launchTransform = transform;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(buttonInput) && canShoot && !isOnCooldown && !isWaitingDelayedShot)
            {
                // If ammo is required, make sure we have enough for this shot
                if (requireAmmo && ammo < ammoCost)
                {
                    onNoAmmo.Invoke();
                    return;
                }

                if (shootDelay <= 0f)
                {
                    // If there's no shoot delay, shoot right away
                    Shoot();
                }
                else
                {
                    // Prevent stacking multiple invokes during delay
                    isWaitingDelayedShot = true;
                    Invoke(nameof(Shoot), shootDelay);
                }
            }
        }

        private void Shoot()
        {
            isWaitingDelayedShot = false;

            if (!canShoot)
            {
                return;
            }

            if (requireAmmo)
            {
                if (ammo < ammoCost)
                {
                    onNoAmmo.Invoke();
                    return;
                }

                ammo -= ammoCost;
                onAmmoChanged.Invoke(ammo);
            }

            // Create a new projectile
            Projectile2D newProjectile = Instantiate(projectile, launchTransform.position, Quaternion.identity);

            if (launchDirection == LaunchDirection.FacingDirection)
            {
                newProjectile.Launch(launchTransform.right, velocity, player);
            }
            else if (launchDirection == LaunchDirection.MousePosition)
            {
                // Get the world position of the mouse cursor
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // Calculate and normalize direction
                Vector2 direction = (mousePosition - (Vector2)launchTransform.position).normalized;
                newProjectile.Launch(direction, velocity, player);
            }

            onProjectileLaunched.Invoke();

            if (cooldown > 0f)
            {
                StartCoroutine(CooldownCoroutine());
            }
        }

        private IEnumerator CooldownCoroutine()
        {
            isOnCooldown = true;
            yield return new WaitForSeconds(cooldown);
            isOnCooldown = false;
        }

        private void OnDisable()
        {
            // Clean up any pending delayed shot when component is disabled
            if (isWaitingDelayedShot)
            {
                CancelInvoke(nameof(Shoot));
                isWaitingDelayedShot = false;
            }
        }

        private void OnValidate()
        {
            // Enforce minimum values in the inspector
            velocity = Mathf.Max(0f, velocity);
            shootDelay = Mathf.Max(0f, shootDelay);
            ammo = Mathf.Max(0, ammo);
            cooldown = Mathf.Max(0, cooldown);
        }
    }
}