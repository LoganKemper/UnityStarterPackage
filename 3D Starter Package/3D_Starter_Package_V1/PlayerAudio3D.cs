using UnityEngine;

/// <summary>
/// Adds sound effects to player movement scripts.
/// </summary>
[RequireComponent(typeof(PlayerMovementBase))]
public class PlayerAudio3D : MonoBehaviour
{
    [Header("Audio Sources")]
    [Tooltip("Assign an AudioSource for the jump sounds.")]
    [SerializeField] private AudioSource jumpSource;

    [Tooltip("Assign an AudioSource for movement sounds. The AudioSource should likely have looping enabled and playOnAwake set to false.")]
    [SerializeField] private AudioSource movementSource;

    [Header("Audio Clips")]
    [Tooltip("Optional: Sound effect for when the player jumps off of the ground.")]
    [SerializeField] private AudioClip jumpSound;

    [Tooltip("Optional: Sound effect for when the player jumps in mid-air.")]
    [SerializeField] private AudioClip midAirJumpSound;

    [Tooltip("Optional: Sound effect for when the player lands on the ground.")]
    [SerializeField] private AudioClip landSound;

    [Tooltip("The downwards velocity needed to trigger the landing sound. The lower this value, the faster the player needs to be falling to play the sound.")]
    [SerializeField] private float landVelocityThreshold = -5f;

    [Tooltip("Optional: Sound effect for the player running. It may be best to have this audio clip seamlessly loop.")]
    [SerializeField] private AudioClip runningSound;

    private PlayerMovementBase playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovementBase>();

        // Subscribe to player movement events
        playerMovement.OnJump += PlayJumpSound;
        playerMovement.OnMidAirJump += PlayDoubleJumpSound;
        playerMovement.OnLand += PlayLandSound;
        playerMovement.OnRunningStateChanged += HandleRunningStateChanged;
    }

    private void OnDestroy()
    {
        // Unsubscribe from events when destroyed
        if (playerMovement != null)
        {
            playerMovement.OnJump -= PlayJumpSound;
            playerMovement.OnMidAirJump -= PlayDoubleJumpSound;
            playerMovement.OnLand -= PlayLandSound;
            playerMovement.OnRunningStateChanged -= HandleRunningStateChanged;
        }
    }

    private void PlayJumpSound()
    {
        if (jumpSound != null)
        {
            jumpSource.PlayOneShot(jumpSound);
        }
    }

    private void PlayDoubleJumpSound()
    {
        if (midAirJumpSound != null)
        {
            jumpSource.PlayOneShot(midAirJumpSound);
        }
    }

    private void PlayLandSound(float velocity)
    {
        if (landSound != null)
        {
            if (velocity <= landVelocityThreshold)
            {
                jumpSource.PlayOneShot(landSound);
            }
        }
    }

    private void HandleRunningStateChanged(bool isRunning)
    {
        if (runningSound == null)
        {
            return;
        }

        if (isRunning)
        {
            if (!movementSource.isPlaying)
            {
                movementSource.clip = runningSound;
                movementSource.Play();
            }
        }
        else
        {
            movementSource.Stop();
        }
    }

    private void OnValidate()
    {
        landVelocityThreshold = Mathf.Min(landVelocityThreshold, 0f);
    }
}
