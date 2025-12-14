// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Add to a GameObject to make it deal damage to other entities.
    /// </summary>
    public class Damager : MonoBehaviour
    {
        public const string ALIGNMENT_DESCRIPTION =
            "Alignment determines who will be affected by this Damager. " +
            "The player will be damaged by Enemy and Environment, but not Player. " +
            "Enemies will be damaged by Player and Environment, but not Enemy.";

        [Header("Read Alignment's Tooltip For Explanation")]
        [Tooltip(ALIGNMENT_DESCRIPTION)]
        public Alignment alignment = Alignment.Player;

        [Header("Damage Settings")]
        [Tooltip("How many points of damage are dealt by this Damager.")]
        public int damage = 1;

        [Tooltip("If true, this Damager will instantly kill the target, regardless of invincibility time.")]
        public bool instakill = false;

        [Tooltip("Enable to make this Damager heal instead of deal damage.")]
        public bool healInstead = false;

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        public void SetInstakill(bool instakill)
        {
            this.instakill = instakill;
        }

        public void SetHealInstead(bool healInstead)
        {
            this.healInstead = healInstead;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            TryToDoDamage(collision);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            TryToDoDamage(collision.collider);
        }

        private void TryToDoDamage(Component target)
        {
            if (!enabled)
            {
                return;
            }

            if (target.TryGetComponent(out IDamageable damageable))
            {
                if (healInstead)
                {
                    damageable.Heal(damage);
                }

                if (damageable.CanBeDamagedBy(alignment))
                {
                    if (instakill)
                    {
                        damageable.Instakill();
                    }
                    else
                    {
                        damageable.DealDamage(damage);
                    }
                }
            }
        }
    }

    public interface IDamageable
    {
        Alignment Alignment { get; }
        bool CanBeDamagedBy(Alignment alignment);
        void DealDamage(int damage);
        void Heal(int health);
        void Instakill();
    }

    public enum Alignment : byte
    {
        Player,
        Enemy,
        Environment
    }
}