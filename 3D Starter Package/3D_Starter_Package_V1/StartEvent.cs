// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

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
}
