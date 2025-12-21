// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Launches projectile attacks towards the player. Can be used for mobile or stationary enemies.
    /// </summary>
    public class EnemyProjectileAttack3D : MonoBehaviour
    {
        public enum FaceTargetType : byte
        {
            DontFace,
            FaceHorizontally,
            FaceDirectly
        }

        [Tooltip("Drag in the projectile prefab.")]
        [SerializeField] private Projectile3D projectile;

        [Tooltip("The position that the projectile should spawn from. If null, this script will use the transform of the GameObject it's attached to.")]
        [SerializeField] private Transform launchTransform;

        [Tooltip("The transform that the projectile will be launched at. If null, this script will try to find the GameObject tagged \"Player\".")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("Choose how this GameObject should turn to face the target.")]
        [SerializeField] private FaceTargetType faceTarget = FaceTargetType.FaceHorizontally;

        [Tooltip("How often a projectile is launched (in seconds).")]
        [SerializeField] private float fireRate = 2f;

        [Tooltip("Adds a random variation of +/- fireRateVariation (in seconds) to the frequency that a projectile is launched. Leave at 0 to ignore.")]
        [SerializeField] private float fireRateVariation = 0f;

        [Tooltip("The initial velocity of the projectile.")]
        [SerializeField] private float velocity = 5f;

        [Tooltip("The maximum distance from this GameObject to the player allowed for projectiles to launch.")]
        [SerializeField] private float maxDistanceFromPlayer = 100f;

        [Space(20)]
        [SerializeField] private UnityEvent onProjectileLaunched;

        private float cooldown;

        // Call this from a UnityEvent to change the target that the projectiles are launched towards
        public void SetTarget(Transform newTarget)
        {
            playerTransform = newTarget;
        }

        public void SetFireRate(float fireRate)
        {
            this.fireRate = fireRate;
        }

        public void SetFireRateVariation(float fireRateVariation)
        {
            this.fireRateVariation = fireRateVariation;
        }

        public void SetVelocity(float velocity)
        {
            this.velocity = velocity;
        }

        public void SetMaxDistanceFromPlayer(float maxDistanceFromPlayer)
        {
            this.maxDistanceFromPlayer = maxDistanceFromPlayer;
        }

        private void Start()
        {
            // If the player's transform has not been assigned, try to find it by tag
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }

            cooldown = GetCooldown();
        }

        private void Update()
        {
            // Return early if the player transform has not been assigned
            if (playerTransform == null)
            {
                return;
            }

            // Subtract the time passed since last frame from the cooldown timer
            cooldown -= Time.deltaTime;

            // If the cooldown has ended and the distance from this GameObject to the player is within range, shoot a projectile
            if (cooldown <= 0 && Vector3.Distance(transform.position, playerTransform.position) <= maxDistanceFromPlayer)
            {
                ShootProjectile();
                cooldown = GetCooldown();
            }
        }

        private void ShootProjectile()
        {
            // Return early if the projectile prefab has not been assigned
            if (projectile == null)
            {
                return;
            }

            // Rotate to face the target
            if (faceTarget != FaceTargetType.DontFace)
            {
                transform.LookAt(playerTransform);

                if (faceTarget == FaceTargetType.FaceHorizontally)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y, 0f));
                }
            }

            Vector3 spawnPosition = launchTransform != null ? launchTransform.position : transform.position;
            Vector3 direction = (playerTransform.position - spawnPosition).normalized;

            // Spawn and launch the projectile
            Projectile3D newProjectile = Instantiate(projectile, spawnPosition, Quaternion.identity);
            newProjectile.Launch(direction, velocity);

            onProjectileLaunched.Invoke();
        }

        // Calculate the cooldown from the fireRate and the fireRateVariation
        private float GetCooldown()
        {
            return fireRateVariation != 0f ? fireRate + Random.Range(-fireRateVariation, fireRateVariation) : fireRate;
        }

        private void OnValidate()
        {
            // Make sure the variables are within acceptable ranges when edited in the inspector
            fireRate = Mathf.Max(0.01f, fireRate);
            maxDistanceFromPlayer = Mathf.Max(0, maxDistanceFromPlayer);
            fireRateVariation = Mathf.Clamp(fireRateVariation, 0, fireRate);
        }
    }
}