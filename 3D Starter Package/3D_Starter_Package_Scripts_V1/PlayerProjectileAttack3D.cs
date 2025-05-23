// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Gives the player a projectile attack.
    /// </summary>
    public class PlayerProjectileAttack3D : MonoBehaviour
    {
        [Tooltip("Drag the projectile prefab in here. The projectile GameObject must have the Projectile3D component on it.")]
        [SerializeField] private Projectile3D projectile;

        [Tooltip("The position that the projectile should spawn from. It's usually a good idea to place it a little in front of the player.")]
        [SerializeField] private Transform launchTransform;

        [Tooltip("The initial velocity of the projectile.")]
        [SerializeField] private float velocity = 25f;

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
                    Shoot();
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
            if (!canShoot || projectile == null)
            {
                return;
            }

            if (requireAmmo)
            {
                ammo--;
                onAmmoChanged.Invoke(ammo);
            }

            // Create a new projectile
            Projectile3D newProjectile = Instantiate(projectile, launchTransform.position, Quaternion.identity);
            newProjectile.Launch(launchTransform, velocity);

            if (shootSound != null)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }

            onProjectileLaunched.Invoke();
        }
    }
}