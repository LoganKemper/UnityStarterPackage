// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds
{
    /// <summary>
    /// Add to a GameObject to make it float up and down and spin.
    /// </summary>
    public class Floater : MonoBehaviour
    {
        [Tooltip("The amplitude (height) of the floating.")]
        [SerializeField] private float amplitude = 0.5f;

        [Tooltip("The frequency (speed) of the floating.")]
        [SerializeField] private float frequency = 1f;

        [Tooltip("The degrees of rotation per second. Set to 0 for no rotation.")]
        [SerializeField] private float degreesOfRotation = 0f;

        [Tooltip("Choose between moving in local or world space.")]
        [SerializeField] private Space space = Space.World;

        private Vector3 positionOffset = new();
        private Vector3 tempPosition = new();

        private void Start()
        {
            // Store the starting position
            positionOffset = transform.position;
        }

        private void Update()
        {
            if (degreesOfRotation != 0f)
            {
                // Spin around the y-axis
                transform.Rotate(new Vector3(0f, Time.deltaTime * degreesOfRotation, 0f), space);
            }

            // Float up and down on a sinewave
            tempPosition = positionOffset;
            tempPosition.y += Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude;
            transform.position = tempPosition;
        }
    }
}