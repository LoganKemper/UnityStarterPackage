using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Gives the player a projectile attack.
/// </summary>
public class PlayerProjectileAttack : MonoBehaviour
{
    [Tooltip("Drag the projectile prefab in here. The projectile GameObject must have the Projectile component on it.")]
    [SerializeField] private Projectile projectile;

    [Tooltip("The position that the projectile should spawn from. It's usually a good idea to place it a little in front of the player.")]
    [SerializeField] private Transform launchTransform;

    [Tooltip("The player's transform. Used to tell which way the player is facing.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("The initial velocity of the projectile.")]
    [SerializeField] private float velocity = 5f;

    [Tooltip("Optional: Delay the spawning of the projectile. Leave at 0 to shoot immediately.")]
    [SerializeField] private float shootDelay = 0f;

    [Tooltip("Whether the projectile attack requires ammunition to work.")]
    [SerializeField] private bool requireAmmo = false;

    [Tooltip("The current quantitiy of ammunition.")]
    [SerializeField] private int ammo = 0;

    [Tooltip("Optional: Sound effect for when the projectile is spawned.")]
    [SerializeField] private AudioClip shootSound;

    [Tooltip("Optional: Sound effect for when a projectile launch is attempted but there is no ammunition.")]
    [SerializeField] private AudioClip noAmmoSound;

    [Space(20)]
    [SerializeField] private UnityEvent onProjectileLaunched;
    [Space(20)]
    [SerializeField] private UnityEvent<int> onAmmoChanged;

    private bool canShoot = true;

    // Call this from a UnityEvent to enable/disable shooting
    public void EnableProjectileAttack(bool enableAttack)
    {
        canShoot = enableAttack;
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
        // "Fire1" is the left mouse button by default
        if (Input.GetButtonDown("Fire1") && canShoot)
        {
            if (!requireAmmo || ammo > 0)
            {
                // Only delay the Shoot method if shootDelay is greater than 0
                if (shootDelay <= 0f)
                {
                    Shoot();
                }
                else
                {
                    // This will invoke the Shoot method after shootDelay seconds
                    Invoke(nameof(Shoot), shootDelay);
                }
            }
            else if (requireAmmo && ammo <= 0)
            {
                if (noAmmoSound != null)
                {
                    AudioSource.PlayClipAtPoint(noAmmoSound, transform.position);
                }
            }
        }
    }

    private void Shoot()
    {
        if (!canShoot)
        {
            return;
        }

        if (requireAmmo)
        {
            ammo--;
            onAmmoChanged.Invoke(ammo);
        }

        // Create a new projectile
        Projectile newProjectile = Instantiate(projectile, launchTransform.position, Quaternion.identity);

        if (playerTransform.localScale.x > 0) // Facing right
        {
            newProjectile.Launch(velocity, true, gameObject);
        }
        else // Facing left
        {
            newProjectile.Launch(-velocity, false, gameObject);
        }

        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound, transform.position);
        }

        onProjectileLaunched.Invoke();
    }
}
