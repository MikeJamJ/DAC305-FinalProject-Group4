using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class SnailAI : MonoBehaviour
{
    public Transform target;    // Target of the snail (i.e the player)
    public Transform snailGFX;  // snail graphics

    public float speed = 4f;                // speed of snail
    public float nextWaypointDistance = 3f; // distance to next waypoint before moving towards it

    private Path path;                      // path from the enemy to the target
    private int currentWaypoint = 0;        // current position in the path array
    private bool reachedEndOfPath = false;  // boolean for reaching the end of path

    private Seeker seeker;      // for pathfinding
    private Rigidbody2D body;   // rigidbody of snail

    void Start()
    {
        // Initialze character objects on startup
        seeker = GetComponent<Seeker>();
        body = GetComponent<Rigidbody2D>();

        // Repeat the function "UpdatePath"
        // start repeating after: 0f seconds
        // repeat every: 0.5f seconds
        InvokeRepeating("UpdatePath", 0f, 0.5f);
    }

    
    // Update is called at a fixed interval
    void FixedUpdate()
    {
        if (path == null)
            return;

        if (currentWaypoint >= path.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        }
        else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - body.position).normalized;

        Vector2 force = direction * speed * Time.deltaTime;

        body.AddForce(force);

        float distance = Vector2.Distance(body.position, path.vectorPath[currentWaypoint]);

        if (distance < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Flip enemy sprite based on direction
        if (body.velocity.x >= 0.01f) {
            snailGFX.localScale = new Vector3(1f, 1f, 1f);
        } else if (body.velocity.x <= -0.01f) {
            snailGFX.localScale = new Vector3(-1f, 1f, 1f);
        }

    }

    // Function for updating the path of the snail
    void UpdatePath() {
        // Only start a path if the ;ast path has finished calculating
        if (seeker.IsDone()) {
            // Make a path from snail to player
            // call function "OnPathComplete" when path is done
            seeker.StartPath(body.position, target.position, OnPathComplete);
        }
        
    }

    // Function for handling when a path is complete
    void OnPathComplete(Path p)
    {   
        // Check that there was no error when making the path
        if (!p.error)
        {   
            // Set new path
            path = p;
            currentWaypoint = 0;
        }
    }
}
