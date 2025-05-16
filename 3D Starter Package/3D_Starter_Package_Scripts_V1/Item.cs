// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Add to a GameObject with a trigger collider to allow the player to add the item to their inventory.
    /// </summary>
    public class Item : MonoBehaviour
    {
        public ItemData itemData;
        public AudioClip pickupSound;
        public bool destroyOnPickup = true;
        public bool isUnique;
    }

    /// <summary>
    /// Holds an item's name and sprite.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        public string name = "";
        public Sprite sprite = null;
    }
}