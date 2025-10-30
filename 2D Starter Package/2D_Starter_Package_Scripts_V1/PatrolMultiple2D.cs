// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace DigitalWorlds.StarterPackage2D
{
    /// <summary>
    /// Moves a GameObject to its assigned waypoints. Can be used for platforms, NPCs, hazards, and more.
    /// </summary>
    public class PatrolMultiple2D : MonoBehaviour
    {
        [System.Serializable]
        public class AnimationParameters
        {
            [Tooltip("Bool parameter: " + nameof(IsRunning))]
            public string IsRunning = "IsRunning";

            [Tooltip("Float parameter: " + nameof(InputX))]
            public string InputX = "InputX";

            [Tooltip("Float parameter: " + nameof(InputY))]
            public string InputY = "InputY";

            [Tooltip("Float parameter: " + nameof(LastInputX))]
            public string LastInputX = "LastInputX";

            [Tooltip("Float parameter: " + nameof(LastInputY))]
            public string LastInputY = "LastInputY";
        }

        public enum PatrolType : byte
        {
            Loop,
            PingPong,
            Neither
        }

        [Header("Patrol System")]
        [Tooltip("The patrolling GameObject will move to each waypoint in order.")]
        [SerializeField] protected List<Transform> waypoints = new();

        [Tooltip("Choose whether the patrolling GameObject should loop through the waypoints forever, ping pong from the first to the last waypoint, or just move through them and stop.")]
        [SerializeField] protected PatrolType patrolType = PatrolType.Loop;

        [Tooltip("How fast the patrolling GameObject will move.")]
        [SerializeField] protected float patrolSpeed = 2f;

        [Tooltip("Optional: Add a pause (in seconds) at each waypoint before moving to the next one. Leave at 0 to ignore.")]
        [SerializeField] protected float pauseAtWaypoint = 1f;

        [Tooltip("How close this GameObject needs to be to the current waypoint in order to move to the next one. May need to be adjusted depending on the scale of your game.")]
        [SerializeField] protected float distanceThreshold = 0.05f;

        [Tooltip("If true, this GameObject will flip its scale on the x-axis to face the next waypoint. Best used in side-scrollers without a blend tree.")]
        [SerializeField] protected bool flipToFaceNextWaypoint = true;

        [Header("Animation")]
        [Tooltip("Optional: Drag in an animator with two blend trees (idle and running). " +
            "Idle uses " + nameof(AnimationParameters.LastInputX) + "/" + nameof(AnimationParameters.LastInputY) + 
            ", running uses " + nameof(AnimationParameters.InputX) + "/" + nameof(AnimationParameters.InputY) + ". " +
            nameof(AnimationParameters.IsRunning) + " switches between them.")]
        [SerializeField] private Animator animator;
        [SerializeField] private AnimationParameters animationParameters;

        protected const float MAGNITUDE_THRESHOLD = 0.0001f;

        private Coroutine patrolCoroutine;
        private bool doPatrol = true;
        private int currentIndex = 0;
        private int direction = 1;
        private int currentFacingDirection;

        // Can be called from other scripts or UnityEvents to stop patrolling
        public void StopPatrolling()
        {
            doPatrol = false;

            if (patrolCoroutine != null)
            {
                StopCoroutine(patrolCoroutine);
                patrolCoroutine = null;
            }

            UpdateAnimator(Vector2.zero, false);
        }

        // Can be called from other scripts or UnityEvents to start or resume patrolling
        public void StartPatrolling()
        {
            if (!doPatrol)
            {
                doPatrol = true;
                patrolCoroutine = StartCoroutine(PatrolRoutine());
            }
        }

        protected virtual void Start()
        {
            // Store the initial facing direction (1 = right, -1 = left)
            currentFacingDirection = transform.localScale.x >= 0 ? 1 : -1;

            // Start this GameObject at the first waypoint and begin patrolling
            if (waypoints.Count > 0)
            {
                transform.position = waypoints[0].position;
                patrolCoroutine = StartCoroutine(PatrolRoutine());
            }
        }

        private IEnumerator PatrolRoutine()
        {
            while (doPatrol)
            {
                // Stop coroutine if there are fewer than two waypoints
                if (waypoints.Count < 2)
                {
                    yield break;
                }

                Transform target = waypoints[currentIndex];

                if (flipToFaceNextWaypoint)
                {
                    FlipIfNeeded(target);
                }

                while (Vector2.Distance(transform.position, target.position) > distanceThreshold && doPatrol)
                {
                    // Update animator to face the target
                    Vector2 toTarget = (target.position - transform.position);
                    Vector2 direction = toTarget.sqrMagnitude > MAGNITUDE_THRESHOLD ? toTarget.normalized : Vector2.zero;
                    UpdateAnimator(direction, true);

                    // Move
                    transform.position = Vector2.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
                    
                    yield return null;
                }

                UpdateAnimator(Vector2.zero, false);

                // Optionally pause movement at each waypoint
                if (pauseAtWaypoint > 0f)
                {
                    yield return new WaitForSeconds(pauseAtWaypoint);
                }

                currentIndex += direction;

                if (patrolType == PatrolType.PingPong)
                {
                    if (currentIndex >= waypoints.Count - 1 || currentIndex <= 0)
                    {
                        direction *= -1;
                    }
                }
                else if (patrolType == PatrolType.Loop)
                {
                    if (currentIndex >= waypoints.Count)
                    {
                        currentIndex = 0;
                    }
                }
                else
                {
                    if (currentIndex >= waypoints.Count)
                    {
                        UpdateAnimator(Vector2.zero, false);
                        yield break;
                    }
                }
            }

            UpdateAnimator(Vector2.zero, false);
        }

        protected void FlipIfNeeded(Transform target)
        {
            float targetDirection = target.position.x - transform.position.x;
            int newFacingDirection = targetDirection >= 0 ? 1 : -1;

            // Only flip if the direction is different from the current facing direction
            if (newFacingDirection != currentFacingDirection)
            {
                Vector3 scale = transform.localScale;
                scale.x = Mathf.Abs(scale.x) * newFacingDirection;
                transform.localScale = scale;
                currentFacingDirection = newFacingDirection;
            }
        }

        protected void UpdateAnimator(Vector2 inputDirection, bool isRunning)
        {
            if (animator == null)
            {
                return;
            }

            // Update input animation parameters
            animator.SetBool(animationParameters.IsRunning, isRunning);
            animator.SetFloat(animationParameters.InputX, inputDirection.x);
            animator.SetFloat(animationParameters.InputY, inputDirection.y);

            // Update last input parameters if currently moving
            if (isRunning && inputDirection.sqrMagnitude > MAGNITUDE_THRESHOLD)
            {
                animator.SetFloat(animationParameters.LastInputX, inputDirection.x);
                animator.SetFloat(animationParameters.LastInputY, inputDirection.y);
            }
        }

        // Draws lines and icons in the scene view to visualizing the patrol path
        private void OnDrawGizmos()
        {
            // Only draw gizmos if there are two or more waypoints
            if (waypoints == null || waypoints.Count < 2)
            {
                return;
            }

            if (Camera.current == null)
            {
                return;
            }

            // Fixed gizmo size at any scale
            Vector3 screenPosition = Camera.current.WorldToScreenPoint(transform.position) + Vector3.right * 10f;
            Vector3 worldPosition = Camera.current.ScreenToWorldPoint(screenPosition);
            float worldSize = (worldPosition - transform.position).magnitude;

            // Draw a line connecting each waypoint
            for (int i = 0; i < waypoints.Count - 1; i++)
            {
                if (waypoints[i] != null && waypoints[i + 1] != null)
                {
                    Gizmos.color = i == 0 && waypoints.Count > 2 ? Color.green : Color.yellow;
                    Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
                }
            }

            // If the patrol loops, draw a line from the last waypoint to the first one
            if (patrolType == PatrolType.Loop && waypoints[0] != null && waypoints[waypoints.Count - 1] != null)
            {
                Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
            }

            // Draw a green circle at the beginning of the path
            if (waypoints[0] != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(waypoints[0].position, worldSize);
            }

            // If the patrol neither loops nor ping pongs, draw a square at the last waypoint
            if (patrolType == PatrolType.Neither && waypoints[waypoints.Count - 1] != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawCube(waypoints[waypoints.Count - 1].position, 2 * worldSize * Vector3.one);
            }
        }

        // Enforce minimum values in the inspector
        protected virtual void OnValidate()
        {
            patrolSpeed = Mathf.Max(0, patrolSpeed);
            pauseAtWaypoint = Mathf.Max(0, pauseAtWaypoint);
            distanceThreshold = Mathf.Max(0, distanceThreshold);
        }
    }
}