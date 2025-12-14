// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Deals damage to players and enemies continuously over time.
    /// </summary>
    public class DamageOverTime2D : MonoBehaviour
    {
        [Tooltip(Damager.ALIGNMENT_DESCRIPTION)]
        [SerializeField] private Alignment alignment = Alignment.Environment;

        [Tooltip("How many points of damage are dealt.")]
        [SerializeField] private int damage = 1;

        [Tooltip("Time (in seconds) between damage ticks.")]
        [SerializeField] private float damageInterval = 1f;

        private readonly HashSet<IDamageable> damageables = new();
        private Coroutine damageCoroutine;

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        public void SetDamageInterval(int damageInterval)
        {
            this.damageInterval = damageInterval;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Add damageable entities to the set
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageables.Add(damageable);

                // If the damage coroutine is not running, run it
                damageCoroutine ??= StartCoroutine(DamageCoroutine());
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // Remove damageable entities from the set
            if (collision.TryGetComponent(out IDamageable damageable))
            {
                damageables.Remove(damageable);
            }
        }

        private IEnumerator DamageCoroutine()
        {
            while (damageables.Count > 0)
            {
                IDamageable[] targets = new IDamageable[damageables.Count];
                damageables.CopyTo(targets);

                foreach (IDamageable damageable in targets)
                {
                    if (damageable != null && damageable.CanBeDamagedBy(alignment))
                    {
                        damageable.DealDamage(damage);
                    }
                }

                yield return new WaitForSeconds(damageInterval);
            }

            damageCoroutine = null;
        }
    }
}