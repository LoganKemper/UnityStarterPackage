// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Rotates the GameObject by a specified amount on each axis.
    /// </summary>
    public class Rotator : MonoBehaviour
    {
        [Tooltip("Add a positive or negative rotation amount to each axis.")]
        [SerializeField] private Vector3 rotation;

        [Tooltip("Choose between rotating in local or world space.")]
        [SerializeField] private Space rotationSpace = Space.Self;

        private void Update()
        {
            transform.Rotate(rotation * Time.deltaTime, rotationSpace);
        }
    }
}