using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Launches projectile attacks towards the player. Can be used for mobile or stationary enemies.
/// </summary>
public class EnemyProjectileAttack : MonoBehaviour
{
    [Tooltip("Drag in the projectile prefab.")]
    [SerializeField] private Projectile projectile;

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

    [SerializeField] private ProjectileDirection projectileDirection = ProjectileDirection.AnyDirection;

    [Space(20)]
    [SerializeField] private UnityEvent onProjectileLaunched;

    public enum ProjectileDirection
    {
        AnyDirection,
        HorizontalOnly,
        VerticalOnly
    }

    private float cooldown;
    private int initialFacingDirection;

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

        // Store the initial facing direction (1 = right, -1 = left)
        initialFacingDirection = transform.localScale.x >= 0 ? 1 : -1;
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

        Projectile newProjectile;
        Vector2 direction;

        // Choose where to launch the projectile from
        Vector3 spawnPosition = launchTransform != null ? launchTransform.position : transform.position;

        // Find the normalized direction of the target
        direction = (playerTransform.position - spawnPosition).normalized;

        if (flipToFacePlayer)
        {
            FlipToFacePlayer();
        }

        // Optionally clamp the projectile horizontally or vertically
        if (projectileDirection == ProjectileDirection.HorizontalOnly)
        {
            direction = new Vector2(direction.x, 0f).normalized;
        }
        else if (projectileDirection == ProjectileDirection.VerticalOnly)
        {
            direction = new Vector2(0f, direction.y).normalized;
        }

        // Spawn the new projectile and launch it
        newProjectile = Instantiate(projectile, spawnPosition, Quaternion.identity);
        newProjectile.Launch(direction, velocity, gameObject);

        onProjectileLaunched.Invoke();
    }

    private void FlipToFacePlayer()
    {
        float playerDirection = playerTransform.position.x - transform.position.x;
        int newFacingDirection = playerDirection >= 0 ? 1 : -1;

        // Only flip if the direction is different from the current facing direction
        if (newFacingDirection != initialFacingDirection)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Abs(scale.x) * newFacingDirection;
            transform.localScale = scale;
        }
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
