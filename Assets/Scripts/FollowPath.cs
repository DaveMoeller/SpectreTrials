using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FollowPath : MonoBehaviour
{
    public Transform[] wayPoints; // Assign in the Inspector
    [Range(.1f, 4.0f)]
    public float moveSpeed = 2.0f;
    private int currentWaypointIndex = 0;
    private int directionToMove = 1; //-1 = retrace steps
    void Update()
    {
        if (currentWaypointIndex < wayPoints.Length)
        {
            // Move towards the current target waypoint
            transform.position = Vector2.MoveTowards(transform.position, wayPoints[currentWaypointIndex].position, moveSpeed * Time.deltaTime);

            // Check if the object is close enough to the waypoint
            if (Vector2.Distance(transform.position, wayPoints[currentWaypointIndex].position) < 0.1f)
            {
                if ((currentWaypointIndex == 0) || (currentWaypointIndex == (wayPoints.Length - 1)))
                {
                    directionToMove *= -1; // Move to the next waypoint
                }
                currentWaypointIndex += directionToMove; // Move to the next waypoint
                if (currentWaypointIndex < 0)
                {
                    currentWaypointIndex = 0;
                }
                if (currentWaypointIndex > (wayPoints.Length - 1))
                {
                    currentWaypointIndex = (wayPoints.Length - 1);
                }
            }
        }
        // Optional: loop the path or destroy the object when finished
    }
}