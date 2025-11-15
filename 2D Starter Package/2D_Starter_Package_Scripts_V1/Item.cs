// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Add to a GameObject with a trigger collider to allow the player to add the item to their inventory.
    /// </summary>
    public class Item : MonoBehaviour
    {
        public ItemData itemData;

        [Tooltip("If true, this GameObject will be destroyed when the item has been picked up.")]
        public bool destroyOnPickup = true;

        [Tooltip("If true, only one of this item is allowed in the inventory at once.")]
        public bool isUnique;

        [Space(20)]
        public UnityEvent onPickedUp;

        public void SetDestroyOnPickup(bool destroyOnPickup)
        {
            this.destroyOnPickup = destroyOnPickup;
        }

        public void SetIsUnique(bool isUnique)
        {
            this.isUnique = isUnique;
        }

        public void SetItemDataName(string name)
        {
            itemData.name = name;
        }

        public void SetItemDataSprite(Sprite sprite)
        {
            itemData.sprite = sprite;
        }
    }

    /// <summary>
    /// Holds an item's name and sprite.
    /// </summary>
    [System.Serializable]
    public class ItemData
    {
        [Tooltip("The item's name (case sensitive).")]
        public string name = "";

        [Tooltip("Optional: Sprite for displaying the item on the UI.")]
        public Sprite sprite = null;
    }
}