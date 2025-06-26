// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// PatrolChase inherits from PatrolMultiple to extend its functionality and add a chasing state.
    /// </summary>
    public class PatrolChase2D : PatrolMultiple2D
    {
        [Header("Chase")]
        [Tooltip("Drag in the player GameObject. If the player has the PlayerStealth2D component on it, PatrolChase2D will check if the player is in cover before chasing them.")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("How fast the GameObject should move towards the player.")]
        [SerializeField] private float chaseSpeed = 4f;

        [Tooltip("The distance at which this GameObject should stop patrolling and begin chasing the player.")]
        [SerializeField] private float detectionRange = 10f;

        [Tooltip("The distance from the player at which this GameObject should stop chasing. Set to 0 and this GameObject will try to ram itself into the player.")]
        [SerializeField] private float stoppingThreshold = 1f;

        [Tooltip("How many seconds to wait after losing the player before resuming patrol.")]
        [SerializeField] private float pauseBeforePatrolling = 1f;

        // This "buffer" is used to prevent rapid switching between chasing and idle states
        private const float DISTANCE_BUFFER = 1.2f;

        private PlayerStealth2D playerStealth;
        private Coroutine returnToPatrolCoroutine;

        protected override void Start()
        {
            base.Start();

            // If playerTransform is not assigned, try to find it by tag
            if (playerTransform == null)
            {
                GameObject playerGameObject = GameObject.FindGameObjectWithTag("Player");
                if (playerGameObject != null)
                {
                    playerTransform = playerGameObject.transform;
                }
            }

            // Attempt to get the PlayerStealth component
            if (playerTransform != null)
            {
                playerStealth = playerTransform.GetComponent<PlayerStealth2D>();
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

            // If in the detection range and not in the stopping threshold, chase the player
            bool shouldChase = distanceToPlayer < detectionRange && IsPlayerVisible();
            bool shouldStopChasing = distanceToPlayer > detectionRange * DISTANCE_BUFFER || !IsPlayerVisible();

            if (shouldChase)
            {
                // Cancel pending patrol resume if chasing
                if (returnToPatrolCoroutine != null)
                {
                    StopCoroutine(returnToPatrolCoroutine);
                    returnToPatrolCoroutine = null;
                }

                if (distanceToPlayer > stoppingThreshold)
                {
                    ChasePlayer();
                }
            }
            else if (shouldStopChasing && returnToPatrolCoroutine == null)
            {
                returnToPatrolCoroutine = StartCoroutine(WaitThenReturnToPatrol());
            }
        }

        private void ChasePlayer()
        {
            // Stop following the patrol path
            StopPatrolling();

            if (flipToFaceNextWaypoint)
            {
                // Turn to face the player (if not already facing that direction)
                FlipIfNeeded(playerTransform);
            }

            // Move directly towards the player at the chase speed
            transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        }

        private bool IsPlayerVisible()
        {
            // If there's no stealth script, treat player as always visible
            return playerStealth == null || !playerStealth.InCover;
        }

        private IEnumerator WaitThenReturnToPatrol()
        {
            yield return new WaitForSeconds(pauseBeforePatrolling);
            StartPatrolling();
            returnToPatrolCoroutine = null;
        }
    }
}