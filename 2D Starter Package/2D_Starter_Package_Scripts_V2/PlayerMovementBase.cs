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

        protected virtual void OnJumpEvent()
        {
            OnJump?.Invoke();
        }

        protected virtual void OnMidAirJumpEvent()
        {
            OnMidAirJump?.Invoke();
        }

        protected virtual void OnLandEvent()
        {
            OnLand?.Invoke();
        }

        protected virtual void OnRunningStateChangedEvent(bool isRunning)
        {
            OnRunningStateChanged?.Invoke(isRunning);
        }

        protected virtual void OnDashEvent()
        {
            OnDash?.Invoke();
        }
    }
}