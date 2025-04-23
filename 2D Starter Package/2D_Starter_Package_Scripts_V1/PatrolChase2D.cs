// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

/// <summary>
/// PatrolChase inherits from PatrolMultiple to extend its functionality and add a chasing state.
/// </summary>
public class PatrolChase2D : PatrolMultiple2D
{
    [Header("Chase")]
    [Tooltip("Drag in the player GameObject.")]
    [SerializeField] private Transform playerTransform;

    [Tooltip("How fast the GameObject should move towards the player.")]
    [SerializeField] private float chaseSpeed = 4f;

    [Tooltip("The distance at which this GameObject should stop patrolling and begin chasing the player.")]
    [SerializeField] private float detectionRange = 10f;

    [Tooltip("The distance from the player at which this GameObject should stop chasing. Set to 0 and this GameObject will try to ram itself into the player.")]
    [SerializeField] private float stoppingThreshold = 1f;

    // This "buffer" is used to prevent rapid switching between chasing and idle states
    private const float DISTANCE_BUFFER = 1.2f;

    protected override void Start()
    {
        base.Start();

        // If playerTransform is not assigned, try to find it by tag
        if (playerTransform == null)
        {
            playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    private void Update()
    {
        // If playerTransform is still not assigned, return early
        if (playerTransform == null)
        {
            return;
        }

        // Get the distance from this GameObject to the player
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);

        if (distanceToPlayer < detectionRange)
        {
            if (distanceToPlayer > stoppingThreshold)
            {
                // If in the detection range and not in the stopping threshold, chase the player
                ChasePlayer();
            }
            else if (distanceToPlayer < stoppingThreshold * DISTANCE_BUFFER)
            {
                // If inside the stopping threshold, go idle
            }
        }
        else if (distanceToPlayer > detectionRange * DISTANCE_BUFFER)
        {
            // If out of the detection range, go back to patrolling
            StartPatrolling();
        }
    }

    private void ChasePlayer()
    {
        // Stop following the patrol path
        StopPatrolling();

        // Turn to face the player (if not already facing that direction)
        FlipIfNeeded(playerTransform);

        // Move directly towards the player at the chase speed
        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
    }
}
