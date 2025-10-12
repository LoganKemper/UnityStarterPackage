// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A simple component for invoking a UnityEvent from another UnityEvent. Use to group and organize complex event sequences.
    /// </summary>
    public class EventRelay : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEvent;

        [ContextMenu("Invoke Event")]
        public void InvokeEvent()
        {
            onEvent.Invoke();
        }
    }
}