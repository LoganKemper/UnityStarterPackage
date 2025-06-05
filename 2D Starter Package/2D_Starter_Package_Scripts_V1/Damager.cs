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
        [Tooltip("This determines who will be damaged by this Damager.\n\n" +
            "The player will be damaged by Enemy and Environment, but not Player. " +
            "Enemies will be damaged by Player and Environment, but not Enemy")]
        public Alignment alignment = Alignment.Player;

        [Tooltip("How many points of damage is dealt by this Damager.")]
        public int damage = 1;
    }

    public enum Alignment
    {
        Player,
        Enemy,
        Environment
    }
}