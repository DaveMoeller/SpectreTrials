using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    public Transform[] waypoints; // Assign in the Inspector
    public float moveSpeed = 5f;
    private int currentWaypointIndex = 0;
    private int directionToMove = 1; //-1 = retrace steps
    void Update()
    {
        if (currentWaypointIndex < waypoints.Length)
        {
            // Move towards the current target waypoint
            transform.position = Vector2.MoveTowards(transform.position, waypoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);

            // Check if the object is close enough to the waypoint
            if (Vector2.Distance(transform.position, waypoints[currentWaypointIndex].position) < 0.1f)
            {
                currentWaypointIndex += directionToMove; // Move to the next waypoint
            }
        }
        // Optional: loop the path or destroy the object when finished
    }
}