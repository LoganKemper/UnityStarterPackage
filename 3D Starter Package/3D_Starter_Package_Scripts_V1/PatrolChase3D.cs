// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;

namespace DigitalWorlds.StarterPackage3D
{
    /// <summary>
    /// PatrolChase3D inherits from PatrolMultiple3D to extend its functionality and add a chasing state.
    /// </summary>
    public class PatrolChase3D : PatrolMultiple3D
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
            float distanceToPlayer = Vector3.Distance(transform.position, playerTransform.position);

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

            // Move directly towards the player at the chase speed
            transform.position = Vector3.MoveTowards(transform.position, playerTransform.position, chaseSpeed * Time.deltaTime);
        }

        protected override void OnValidate()
        {
            base.OnValidate();
            chaseSpeed = Mathf.Max(0, chaseSpeed);
            detectionRange = Mathf.Max(0, detectionRange);
            stoppingThreshold = Mathf.Max(0, stoppingThreshold);
        }
    }
}