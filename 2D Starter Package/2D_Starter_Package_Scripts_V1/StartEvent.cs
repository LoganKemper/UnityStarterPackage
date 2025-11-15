// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Generic script for adding a UnityEvent to the Start method.
    /// </summary>
    public class StartEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onStartEvent;

        private void Start()
        {
            onStartEvent.Invoke();
        }

        [ContextMenu("Invoke Event")]
        public void InvokeEvent()
        {
            onStartEvent.Invoke();
        }
    }
}