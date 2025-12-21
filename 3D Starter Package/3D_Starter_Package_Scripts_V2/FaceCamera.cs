// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// Attach to a GameObject to make it always face towards the main camera.
    /// </summary>
    public class FaceCamera : MonoBehaviour
    {
        [Tooltip("If true, the object will face away from the camera instead of toward it.")]
        [SerializeField] private bool flipForward = false;

        [Tooltip("How quickly the object rotates to face the camera. 0 = instant.")]
        [SerializeField] private float facingSpeed = 0f;

        private void Update()
        {
            if (Camera.main == null)
            {
                return;
            }

            Vector3 direction = Camera.main.transform.position - transform.position;

            if (flipForward)
            {
                direction = -direction;
            }

            if (direction.sqrMagnitude < 0.0001f)
            {
                return;
            }

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);

            if (facingSpeed <= 0f)
            {
                // Instant snap
                transform.rotation = targetRotation;
            }
            else
            {
                // Smooth rotate
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, facingSpeed * Time.deltaTime);
            }
        }
    }
}