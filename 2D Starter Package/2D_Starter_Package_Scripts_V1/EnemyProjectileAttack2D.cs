// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Launches projectile attacks towards the player. Can be used for mobile or stationary enemies.
    /// </summary>
    public class EnemyProjectileAttack2D : MonoBehaviour
    {
        public enum ProjectileDirection : byte
        {
            AnyDirection,
            HorizontalOnly,
            VerticalOnly,
            FourDirections,
            EightDirections
        }

        [Tooltip("Drag in the projectile prefab.")]
        [SerializeField] private Projectile2D projectile;

        [Tooltip("The position that the projectile should spawn from. If null, this script will use the transform of the GameObject it's attached to.")]
        [SerializeField] private Transform launchTransform;

        [Tooltip("The transform that the projectile will be launched at. If null, this script will try to find the GameObject tagged \"Player\".")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("If true, this will flip this GameObject's scale on the x-axis to face the player when launching a projectile.")]
        [SerializeField] private bool flipToFacePlayer = true;

        [Tooltip("How often a projectile is launched (in seconds).")]
        [SerializeField] private float fireRate = 2f;

        [Tooltip("Adds a random variation of +/- fireRateVariation (in seconds) to the frequency that a projectile is launched. Leave at 0 to ignore.")]
        [SerializeField] private float fireRateVariation = 0f;

        [Tooltip("The initial velocity of the projectile.")]
        [SerializeField] private float velocity = 5f;

        [Tooltip("The maximum distance from this GameObject to the player allowed for projectiles to launch.")]
        [SerializeField] private float maxDistanceFromPlayer = 100f;

        [Tooltip("Choose which directions the projectile can be launched in.")]
        [SerializeField] private ProjectileDirection projectileDirection = ProjectileDirection.AnyDirection;

        [Space(20)]
        [SerializeField] private UnityEvent onProjectileLaunched;

        private float cooldown;
        private int currentFacingDirection; // -1 = left, 1 = right

        // Call this from a UnityEvent to change the target that the projectiles are launched towards
        public void SetTarget(Transform newTarget)
        {
            playerTransform = newTarget;
        }

        private void Start()
        {
            // If the player's transform has not been assigned, try to find it by tag
            if (playerTransform == null)
            {
                playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
            }

            cooldown = GetCooldown();

            // Store the initial facing direction
            currentFacingDirection = transform.localScale.x >= 0 ? 1 : -1;
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
            if (cooldown <= 0 && Vector2.Distance(transform.position, playerTransform.position) <= maxDistanceFromPlayer)
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

            // Choose where to launch the projectile from
            Vector3 spawnPosition = launchTransform != null ? launchTransform.position : transform.position;

            // Find the normalized direction of the target
            Vector2 direction = (playerTransform.position - spawnPosition).normalized;

            if (flipToFacePlayer)
            {
                FlipToFacePlayer();
            }

            // Projectile direction clamp modes
            switch (projectileDirection)
            {
                case ProjectileDirection.HorizontalOnly:
                    direction = new Vector2(direction.x, 0f).normalized;
                    break;

                case ProjectileDirection.VerticalOnly:
                    direction = new Vector2(0f, direction.y).normalized;
                    break;

                case ProjectileDirection.FourDirections:
                    direction = SnapToDirectionSet(direction, FOUR_DIRECTIONS_SET);
                    break;

                case ProjectileDirection.EightDirections:
                    direction = SnapToDirectionSet(direction, EIGHT_DIRECTIONS_SET);
                    break;

                case ProjectileDirection.AnyDirection:
                default:
                    break;
            }

            // Spawn the new projectile and launch it
            Projectile2D newProjectile = Instantiate(projectile, spawnPosition, Quaternion.identity);
            newProjectile.Launch(direction, velocity, gameObject);

            onProjectileLaunched.Invoke();
        }

        private void FlipToFacePlayer()
        {
            float playerDirection = playerTransform.position.x - transform.position.x;
            int newFacingDirection = playerDirection >= 0 ? 1 : -1;

            // Only flip if the direction is different from the current facing direction
            if (newFacingDirection != currentFacingDirection)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * newFacingDirection;
                transform.localScale = scale;
                currentFacingDirection = newFacingDirection;
            }
        }

        // Calculate the cooldown from the fireRate and the fireRateVariation
        private float GetCooldown()
        {
            return fireRateVariation != 0f ? fireRate + Random.Range(-fireRateVariation, fireRateVariation) : fireRate;
        }

        private Vector2 SnapToDirectionSet(Vector2 vector, Vector2[] directionsSet)
        {
            if (vector.sqrMagnitude <= Mathf.Epsilon)
            {
                // Default to first if vector is zero
                return directionsSet[0];
            }

            vector.Normalize();
            float bestDot = float.NegativeInfinity;
            Vector2 bestVector = directionsSet[0];

            // Iterate through the set of vectors to find the best match
            for (int i = 0; i < directionsSet.Length; i++)
            {
                // Dot product finds the angle between two vectors
                float dotProduct = Vector2.Dot(vector, directionsSet[i]);
                if (dotProduct > bestDot)
                {
                    bestDot = dotProduct;
                    bestVector = directionsSet[i];
                }
            }
            return bestVector;
        }

        private static readonly Vector2[] FOUR_DIRECTIONS_SET =
        {
            new( 1f,  0f), // East
            new( 0f,  1f), // North
            new(-1f,  0f), // West
            new( 0f, -1f), // South
        };

        private static readonly float INVERTED_SQRT_OF_2 = 1 / Mathf.Sqrt(2);

        private static readonly Vector2[] EIGHT_DIRECTIONS_SET =
        {
            new( 1f,  0f),                                 // East
            new( INVERTED_SQRT_OF_2,  INVERTED_SQRT_OF_2), // Northeast
            new( 0f,  1f),                                 // North
            new(-INVERTED_SQRT_OF_2,  INVERTED_SQRT_OF_2), // Northwest
            new(-1f,  0f),                                 // West
            new(-INVERTED_SQRT_OF_2, -INVERTED_SQRT_OF_2), // Southwest
            new( 0f, -1f),                                 // South
            new( INVERTED_SQRT_OF_2, -INVERTED_SQRT_OF_2), // Southeast
        };

        private void OnValidate()
        {
            // Make sure the variables are within acceptable ranges when edited in the inspector
            fireRate = Mathf.Max(0.01f, fireRate);
            maxDistanceFromPlayer = Mathf.Max(0, maxDistanceFromPlayer);
            fireRateVariation = Mathf.Clamp(fireRateVariation, 0, fireRate);
        }
    }
}