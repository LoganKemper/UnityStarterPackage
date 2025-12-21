// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Used to add collectables to the CollectableManager.
    /// </summary>
    public class Collectable : MonoBehaviour
    {
        [Tooltip("Name used to identify the collectable. Must match the name in CollectableManager exactly.")]
        [SerializeField] private string collectableName = "Coin";

        [Tooltip("Quantity of this collectable.")]
        [SerializeField] private int collectableCount = 1;

        // Call from a UnityEvent to pick up this collectable
        public void PickUpCollectable()
        {
            if (CollectableManager.Instance == null)
            {
                Debug.LogWarning("CollectableManager not found!");
                return;
            }

            CollectableManager.Instance.AddCollectable(collectableName, collectableCount);
        }

        // Call from a UnityEvent to pick up this collectable and specify the collectable amount
        public void PickUpCollectableAmount(int amount)
        {
            if (CollectableManager.Instance == null)
            {
                Debug.LogWarning("CollectableManager not found!");
                return;
            }

            CollectableManager.Instance.AddCollectable(collectableName, amount);
        }

        // Call from a UnityEvent to set the collectable count to a particular value
        public void SetCollectableTo(int value)
        {
            if (CollectableManager.Instance == null)
            {
                Debug.LogWarning("CollectableManager not found!");
                return;
            }

            CollectableManager.Instance.SetCollectable(collectableName, value);
        }
    }
}