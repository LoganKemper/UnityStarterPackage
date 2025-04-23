// Unity Starter Package - Version 1
// University of Florida's Digital Worlds Institute
// Written by Logan Kemper

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Moves a GameObject to its assigned waypoints. Can be used for platforms, NPCs, hazards, and more.
/// </summary>
public class PatrolMultiple3D : MonoBehaviour
{
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

    [Header("Turn to Face Target")]
    [Tooltip("Choose how the patrolling GameObject should turn to face the next waypoint.")]
    [SerializeField] protected FaceTargetType faceTarget = FaceTargetType.FaceHorizontally;

    [Tooltip("How fast the patrolling GameObject will rotate to face the next waypoint.")]
    [SerializeField] protected float rotationSpeed = 5f;

    private bool doPatrol = true;
    private int currentIndex = 0;
    private int direction = 1;
    private Coroutine patrolCoroutine;

    public enum PatrolType
    {
        Loop,
        PingPong,
        Neither,
    }

    public enum FaceTargetType
    {
        DontFace,
        FaceHorizontally,
        FaceDirectly
    }

    // Can be called from other scripts or UnityEvents to stop patrolling
    public void StopPatrolling()
    {
        doPatrol = false;
        if (patrolCoroutine != null)
        {
            StopCoroutine(patrolCoroutine);
            patrolCoroutine = null;
        }
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

            while (Vector3.Distance(transform.position, target.position) > distanceThreshold)
            {
                // Rotate towards the target
                if (faceTarget != FaceTargetType.DontFace)
                {
                    Vector3 targetDirection = target.position - transform.position;

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

                // Move towards the target
                transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
                yield return null;
            }

            while (Vector3.Distance(transform.position, target.position) > distanceThreshold)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.position, patrolSpeed * Time.deltaTime);
                yield return null;
            }

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
                    yield break;
                }
            }
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

        Gizmos.color = Color.yellow;

        // Draw a line connecting each waypoint
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
            {
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }

        // If the patrol loops, draw a line from the last waypoint to the first one
        if (patrolType == PatrolType.Loop && waypoints[0] != null && waypoints[waypoints.Count - 1] != null)
        {
            Gizmos.DrawLine(waypoints[waypoints.Count - 1].position, waypoints[0].position);
        }

        // Draw a green circle at the beginning of the path
        Gizmos.color = Color.green;

        if (waypoints[0] != null)
        {
            Gizmos.DrawSphere(waypoints[0].position, 0.25f);
        }

        // If the patrol neither loops nor ping pongs, draw a square at the last waypoint
        if (patrolType == PatrolType.Neither && waypoints[waypoints.Count - 1] != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawCube(waypoints[waypoints.Count - 1].position, Vector3.one * 0.5f);
        }
    }

    // Enforce minimum values in the inspector
    protected virtual void OnValidate()
    {
        patrolSpeed = Mathf.Max(0, patrolSpeed);
        rotationSpeed = Mathf.Max(0, rotationSpeed);
        pauseAtWaypoint = Mathf.Max(0, pauseAtWaypoint);
        distanceThreshold = Mathf.Max(0, distanceThreshold);
    }
}
