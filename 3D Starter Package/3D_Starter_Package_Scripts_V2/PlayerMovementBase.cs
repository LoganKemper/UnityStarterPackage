// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System;
using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// A base class for giving shared functionality to player controllers.
    /// </summary>
    public abstract class PlayerMovementBase : MonoBehaviour
    {
        public event Action OnJump;
        public event Action OnMidAirJump;
        public event Action<float> OnLand;
        public event Action<bool> OnRunningStateChanged;

        protected void OnJumpEvent()
        {
            OnJump?.Invoke();
        }

        protected void OnMidAirJumpEvent()
        {
            OnMidAirJump?.Invoke();
        }

        protected void OnLandEvent(float velocity)
        {
            OnLand?.Invoke(velocity);
        }

        protected void OnRunningStateChangedEvent(bool isRunning)
        {
            OnRunningStateChanged?.Invoke(isRunning);
        }
    }
}