using UnityEngine;

/// <summary>
/// A base class for giving shared functionality to player controllers.
/// </summary>
public abstract class PlayerMovementBase : MonoBehaviour
{
    public System.Action OnJump;
    public System.Action OnMidAirJump;
    public System.Action<float> OnLand;
    public System.Action<bool> OnRunningStateChanged;
}
