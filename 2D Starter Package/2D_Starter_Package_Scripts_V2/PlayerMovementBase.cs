// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System;
using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// A base class for giving shared functionality to player controllers.
    /// </summary>
    public abstract class PlayerMovementBase : MonoBehaviour
    {
        // Events for different player actions
        // These are C# events (not UnityEvents) and must be accessed via code
        public event Action OnJump;
        public event Action OnMidAirJump;
        public event Action OnLand;
        public event Action<bool> OnRunningStateChanged;
        public event Action OnDash;

        protected void OnJumpEvent()
        {
            OnJump?.Invoke();
        }

        protected void OnMidAirJumpEvent()
        {
            OnMidAirJump?.Invoke();
        }

        protected void OnLandEvent()
        {
            OnLand?.Invoke();
        }

        protected void OnRunningStateChangedEvent(bool isRunning)
        {
            OnRunningStateChanged?.Invoke(isRunning);
        }

        protected void OnDashEvent()
        {
            OnDash?.Invoke();
        }
    }
}