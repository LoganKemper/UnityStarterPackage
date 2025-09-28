// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Generic script for spawning in GameObjects.
    /// </summary>
    public class Spawner : MonoBehaviour
    {
        [Tooltip("Drag in a GameObject to be copied by the spawner. It could be a prefab from the project assets or a GameObject in the hierarchy.")]
        [SerializeField] private GameObject objectToSpawn;

        [Tooltip("Optional: The transform that the object will be spawned at. If left empty, this GameObject's transform will be used.")]
        [SerializeField] private Transform spawnTransform;

        public void SpawnObject()
        {
            if (objectToSpawn == null)
            {
                Debug.LogWarning("Spawner does not have an object to spawn");
            }
            else
            {
                if (spawnTransform == null)
                {
                    Instantiate(objectToSpawn, transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(objectToSpawn, spawnTransform.position, spawnTransform.rotation);
                }
            }
        }
    }
}