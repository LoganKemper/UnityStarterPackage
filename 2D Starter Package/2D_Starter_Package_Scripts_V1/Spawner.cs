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

        public void SpawnObject()
        {
            if (objectToSpawn == null)
            {
                Debug.LogWarning("Spawner does not have an object to spawn");
            }
            else
            {
                Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            }
        }
    }
}