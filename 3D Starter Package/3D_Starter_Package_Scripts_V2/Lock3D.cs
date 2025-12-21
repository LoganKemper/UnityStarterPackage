// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Add to a GameObject with a trigger collider to create a lock that can only be unlocked if the player has the requisite item(s) in their inventory.
    /// </summary>
    public class Lock3D : MonoBehaviour
    {
        [System.Serializable]
        public class RequiredItem
        {
            [Tooltip("The name of the item that the lock requires.")]
            public string itemName = "Key";

            [Tooltip("The quantity of the item required.")]
            public int itemCount = 1;
        }

        private enum KeyLocation : byte
        {
            Inventory,
            CollectableManager
        }

        [Tooltip("Enter the tag name that should register collisions.")]
        [SerializeField] private string tagName = "Player";

        [Tooltip("Choose whether to check for the required item in the player's inventory or in the collectable manager.")]
        [SerializeField] private KeyLocation keyLocation = KeyLocation.Inventory;

        [SerializeField] private RequiredItem[] requiredItems;

        [Tooltip("Whether a button press should be required to unlock the lock. If false, it will check automatically on the trigger collision.")]
        [SerializeField] private bool requireButtonPress;

        [Tooltip("The key input that the script is listening for.")]
        [SerializeField] private KeyCode keyToPress = KeyCode.E;

        [Tooltip("Whether the items required for the lock should be deleted when unlocking.")]
        [SerializeField] private bool deleteItemsWhenUsed;

        [Space(20)]
        [SerializeField] private UnityEvent onUnlocked, onUnlockFailed;

        private Inventory inventory;

        private void Update()
        {
            if (Input.GetKeyDown(keyToPress) && requireButtonPress && inventory != null)
            {
                CheckUnlock();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                if (other.gameObject.TryGetComponent(out Inventory inv))
                {
                    inventory = inv;

                    if (!requireButtonPress)
                    {
                        CheckUnlock();
                    }
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!string.IsNullOrEmpty(tagName) && other.CompareTag(tagName))
            {
                if (other.gameObject.TryGetComponent(out Inventory inv) && inv == inventory)
                {
                    inventory = null;
                }
            }
        }

        // Check if the required items are in the inventory, then handle the unlocking
        private void CheckUnlock()
        {
            // Treat no requirements as success
            if (requiredItems == null || requiredItems.Length == 0)
            {
                onUnlocked.Invoke();
                return;
            }

            // Check the player's inventory
            if (keyLocation == KeyLocation.Inventory)
            {
                if (inventory == null)
                {
                    onUnlockFailed.Invoke();
                    return;
                }

                // Verify required items
                for (int i = 0; i < requiredItems.Length; i++)
                {
                    RequiredItem requiredItem = requiredItems[i];
                    int needed = Mathf.Max(0, requiredItem.itemCount);
                    if (needed == 0)
                    {
                        continue;
                    }

                    int owned = inventory.GetItemCount(requiredItem.itemName);
                    if (owned < needed)
                    {
                        onUnlockFailed.Invoke();
                        return;
                    }
                }

                if (deleteItemsWhenUsed)
                {
                    for (int i = 0; i < requiredItems.Length; i++)
                    {
                        RequiredItem requiredItem = requiredItems[i];
                        int needed = Mathf.Max(0, requiredItem.itemCount);
                        if (needed > 0)
                        {
                            inventory.DeleteItemFromInventory(requiredItem.itemName, needed);
                        }
                    }
                }

                onUnlocked.Invoke();
                return;
            }

            // Check the collectable manager
            if (keyLocation == KeyLocation.CollectableManager)
            {
                CollectableManager collectableManager = CollectableManager.Instance;
                if (collectableManager == null)
                {
                    onUnlockFailed.Invoke();
                    return;
                }

                // Verify required items
                for (int i = 0; i < requiredItems.Length; i++)
                {
                    RequiredItem requiredItem = requiredItems[i];
                    int needed = Mathf.Max(0, requiredItem.itemCount);
                    if (needed == 0)
                    {
                        continue;
                    }

                    var collectable = collectableManager.FindCollectable(requiredItem.itemName);
                    int owned = (collectable != null) ? collectable.count : 0;
                    if (owned < needed)
                    {
                        onUnlockFailed.Invoke();
                        return;
                    }
                }

                if (deleteItemsWhenUsed)
                {
                    for (int i = 0; i < requiredItems.Length; i++)
                    {
                        RequiredItem requiredItem = requiredItems[i];
                        int needed = Mathf.Max(0, requiredItem.itemCount);
                        if (needed > 0)
                        {
                            collectableManager.AddCollectable(requiredItem.itemName, -needed);
                        }
                    }
                }

                onUnlocked.Invoke();
                return;
            }

            // Fallback
            onUnlockFailed.Invoke();
        }
    }
}