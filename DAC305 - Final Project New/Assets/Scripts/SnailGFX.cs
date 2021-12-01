using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding; // Import A* pathfinding scripts

public class SnailGFX : MonoBehaviour
{

    [SerializeField] private AIPath aIPath;

    // Update is called once per frame
    void Update()
    {   
        // Flip enemy sprite based on direction
        if (aIPath.desiredVelocity.x >= 0.01f) {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        } else if (aIPath.desiredVelocity.x <= -0.01f) {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        
    }
}
