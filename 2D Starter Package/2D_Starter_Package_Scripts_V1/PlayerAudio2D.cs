// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

/// <summary>
/// Adds sound effects to PlayerMovement2D.
/// </summary>
[RequireComponent(typeof(PlayerMovement2D))]
public class PlayerAudio2D : MonoBehaviour
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

    [Tooltip("Optional: Sound effect for the player running. It may be best to have this audio clip seamlessly loop.")]
    [SerializeField] private AudioClip runningSound;

    [Tooltip("Optional: Sound effect for the player dashing.")]
    [SerializeField] private AudioClip dashSound;

    private PlayerMovement2D playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<PlayerMovement2D>();

        if (playerMovement == null)
        {
            Debug.LogWarning("Make sure PlayerAudio2D is attached to a GameObject with PlayerMovement2D!");
        }

        // Subscribe to player movement events
        playerMovement.OnJump += PlayJumpSound;
        playerMovement.OnMidAirJump += PlayDoubleJumpSound;
        playerMovement.OnLand += PlayLandSound;
        playerMovement.OnRunningStateChanged += HandleRunningStateChanged;
        playerMovement.OnDash += PlayDashSound;
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
            playerMovement.OnDash -= PlayDashSound;
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

    private void PlayLandSound()
    {
        if (landSound != null)
        {
            jumpSource.PlayOneShot(landSound);
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

    private void PlayDashSound()
    {
        if (dashSound != null)
        {
            jumpSource.PlayOneShot(dashSound);
        }
    }
}
