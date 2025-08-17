// Unity Starter Package - Version 1
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
        [Header("Read Alignment's Tooltip For Explanation")]
        [Tooltip("Alignment determines who will be affected by this Damager. " +
            "The player will be damaged by Enemy and Environment, but not Player. " +
            "Enemies will be damaged by Player and Environment, but not Enemy.")]
        public Alignment alignment = Alignment.Player;

        [Header("Damage Settings")]
        [Tooltip("How many points of damage is dealt by this Damager.")]
        public int damage = 1;

        [Tooltip("Enable to make this Damager heal instead of deal damage.")]
        public bool healInstead = false;

        public void SetDamage(int damage)
        {
            this.damage = damage;
        }

        public void SetHealInstead(bool healInstead)
        {
            this.healInstead = healInstead;
        }
    }

    public enum Alignment
    {
        Player,
        Enemy,
        Environment
    }
}