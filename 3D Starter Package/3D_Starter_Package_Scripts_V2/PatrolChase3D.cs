// Unity Starter Package - Version 2
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// PatrolChase3D inherits from PatrolMultiple3D to extend its functionality and add a chasing state.
    /// </summary>
    public class PatrolChase3D : PatrolMultiple3D
    {
        [System.Serializable]
        public class PatrolEvents
        {
            [Space(20)]
            public UnityEvent onChasePlayer;

            [Space(20)]
            public UnityEvent onPatrol;
        }

        public enum ChaseMode : byte
        {
            Direct,
            HorizontalOnly,
            VerticalOnly
        }

        [Header("Chase")]
        [Tooltip("Drag in the player GameObject. If the player has the PlayerStealth3D component on it, PatrolChase3D will check if the player is in cover before chasing them.")]
        [SerializeField] private Transform playerTransform;

        [Tooltip("How fast the GameObject should move towards the player.")]
        [SerializeField] private float chaseSpeed = 4f;

        [Tooltip("The distance at which this GameObject should stop patrolling and begin chasing the player.")]
        [SerializeField] private float detectionRange = 10f;

        [Tooltip("The distance from the player at which this GameObject should stop chasing. Set to 0 and this GameObject will try to ram itself into the player.")]
        [SerializeField] private float stoppingThreshold = 1f;

        [Tooltip("How many seconds to wait after losing the player before resuming patrol.")]
        [SerializeField] private float pauseBeforePatrolling = 1f;

        [Tooltip("How the chase behavior should work.")]
        [SerializeField] private ChaseMode chaseMode = ChaseMode.Direct;

        [Header("Patrol Events")]
        [SerializeField] private PatrolEvents patrolEvents;

        // This "buffer" is used to prevent rapid switching between chasing and idle states
        private const float DISTANCE_BUFFER = 1.2f;

        private PlayerStealth3D playerStealth;
        private Coroutine returnToPatrolCoroutine;
        private bool isChasing = true;

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
                playerStealth = playerTransform.GetComponent<PlayerStealth3D>();
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
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

            // If in the detection range and not in the stopping threshold, chase the player
            bool shouldChase = distanceToPlayer < detectionRange && IsPlayerVisible();
            bool shouldStopChasing = distanceToPlayer > detectionRange * DISTANCE_BUFFER || !IsPlayerVisible();

            if (shouldChase)
            {
                if (!isChasing)
                {
                    isChasing = true;
                    patrolEvents.onChasePlayer.Invoke();
                }

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
                if (isChasing)
                {
                    isChasing = false;
                    patrolEvents.onPatrol.Invoke();
                }

                returnToPatrolCoroutine = StartCoroutine(WaitThenReturnToPatrol());
            }
        }

        private void ChasePlayer()
        {
            // Stop following the patrol path
            StopPatrolling();

            // Rotate towards the player
            if (faceTarget != FaceTargetType.DontFace)
            {
                Vector3 targetDirection = playerTransform.position - transform.position;

                if (faceTarget == FaceTargetType.FaceHorizontally)
                {
                    targetDirection.y = 0;
                }

                if (targetDirection != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                }
            }

            // Choose target position based on chase mode
            Vector3 targetPosition = playerTransform.position;

            switch (chaseMode)
            {
                case ChaseMode.Direct:
                    // Use player's full position
                    break;

                case ChaseMode.HorizontalOnly:
                    // Match the target's x/z, keep current y
                    targetPosition.y = transform.position.y;
                    break;

                case ChaseMode.VerticalOnly:
                    // Only move on y, keep current x/z
                    targetPosition.x = transform.position.x;
                    targetPosition.z = transform.position.z;
                    break;
            }

            // Move towards the chosen target position at the chase speed
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);
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

        protected override void OnValidate()
        {
            base.OnValidate();

            detectionRange = Mathf.Max(0, detectionRange);
            stoppingThreshold = Mathf.Max(0, stoppingThreshold);
            pauseBeforePatrolling = Mathf.Max(0, pauseBeforePatrolling);
        }
    }
}