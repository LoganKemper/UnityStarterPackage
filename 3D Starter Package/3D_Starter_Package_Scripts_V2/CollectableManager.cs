// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Manages one or more collectable items and displays them on the UI.
    /// </summary>
    public class CollectableManager : MonoBehaviour
    {
        [System.Serializable]
        public class CollectableDisplay
        {
            [Tooltip("Name used to identify the collectable.")]
            public string collectableName = "Coin";

            [Tooltip("Optional: Prefix written before the count on the UI.")]
            public string collectableTextPrefix = "Coins: ";

            [Tooltip("Optional: Text element to display the collectable to.")]
            public TMP_Text collectableText;

            [Tooltip("Current quantity of the collectable.")]
            public int count = 0;

            [Tooltip("If true, the collectable's value is saved between scenes.")]
            public bool persistentCollectable = false;

            [Space(20)]
            public UnityEvent<int> onCollectableUpdated;
        }

        private static readonly Dictionary<string, int> persistentValues = new();

        // This will reset persistentValues when the game starts
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetPersistentValues()
        {
            persistentValues.Clear();
        }

        [SerializeField] private CollectableDisplay[] collectableDisplays;

        // This class uses the singleton pattern.
        // This means that it can be accessed from other scripts without a direct object reference.
        // There should only be *one* of a given type of singleton in the scene at once.
        // Access this singleton by using CollectableManager.Instance...
        public static CollectableManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.LogWarning($"Duplicate {nameof(CollectableManager)} components found! Destroying this one on: {gameObject.name}");
                Destroy(this);
                return;
            }

            if (collectableDisplays == null)
            {
                return;
            }

            foreach (var collectable in collectableDisplays)
            {
                if (collectable.persistentCollectable && persistentValues.TryGetValue(collectable.collectableName, out int savedValue))
                {
                    collectable.count = savedValue;
                }

                UpdateCollectableText(collectable);
            }
        }

        public void AddCollectable(string name, int count = 1)
        {
            var collectable = FindCollectable(name);
            if (collectable != null)
            {
                collectable.count += count;

                if (collectable.persistentCollectable)
                {
                    persistentValues[collectable.collectableName] = collectable.count;
                }

                UpdateCollectableText(collectable);
                collectable.onCollectableUpdated.Invoke(collectable.count);
            }
            else
            {
                Debug.LogWarning("No collectable named \"" + name + "\" found");
            }
        }

        public void SetCollectable(string name, int count)
        {
            var collectable = FindCollectable(name);
            if (collectable != null)
            {
                collectable.count = count;

                if (collectable.persistentCollectable)
                {
                    persistentValues[collectable.collectableName] = collectable.count;
                }

                UpdateCollectableText(collectable);
                collectable.onCollectableUpdated.Invoke(collectable.count);
            }
            else
            {
                Debug.LogWarning("No collectable named \"" + name + "\" found");
            }
        }

        public CollectableDisplay FindCollectable(string name)
        {
            foreach (var collectable in collectableDisplays)
            {
                if (collectable.collectableName == name)
                {
                    return collectable;
                }
            }

            return null;
        }

        private void UpdateCollectableText(CollectableDisplay collectable)
        {
            if (collectable.collectableText == null)
            {
                return;
            }

            collectable.collectableText.text = collectable.collectableTextPrefix + collectable.count;
        }
    }
}