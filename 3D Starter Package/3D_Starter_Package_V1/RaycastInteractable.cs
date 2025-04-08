// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Used in conjunction with FirstPersonInteractor to add a UnityEvent to interactions.
/// </summary>
public class RaycastInteractable : MonoBehaviour
{
    [SerializeField] private UnityEvent onInteraction;

    public void Interaction()
    {
        onInteraction.Invoke();
    }
}
