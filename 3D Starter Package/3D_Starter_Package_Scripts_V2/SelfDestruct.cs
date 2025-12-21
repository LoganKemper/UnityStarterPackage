// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// SelfDestruct can be used to destroy components and GameObjects via UnityEvents.
    /// </summary>
    public class SelfDestruct : MonoBehaviour
    {
        // Destroys the GameObject that this SelfDestruct component is attached to
        public void DestroyThisGameObject()
        {
            Destroy(gameObject);
        }

        // Destroys the target GameObject
        public void DestroyTargetGameObject(GameObject target)
        {
            Destroy(target);
        }

        // Destroys the GameObject that this SelfDestruct component is attached to after a given number of seconds 
        public void DestroyAfterSeconds(float seconds)
        {
            Destroy(gameObject, seconds);
        }

        // Destroys the target component
        public void DestroyTargetComponent(Component target)
        {
            Destroy(target);
        }

        // Destroys this GameObject after one physics update
        public void DestroyAfterPhysics()
        {
            StartCoroutine(DestroyAfterPhysicsCoroutine());
        }

        private IEnumerator DestroyAfterPhysicsCoroutine()
        {
            yield return new WaitForFixedUpdate();
            Destroy(gameObject);
        }

        // Destroys this GameObject during the next frame of the game
        public void DestroyNextFrame()
        {
            StartCoroutine(DestroyNextFrameCoroutine());
        }

        private IEnumerator DestroyNextFrameCoroutine()
        {
            yield return null;
            Destroy(gameObject);
        }
    }
}