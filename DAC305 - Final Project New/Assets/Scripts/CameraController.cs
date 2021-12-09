using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{

    [SerializeField] private GameObject player; // player game object that we will follow

    private Transform playerTransform; // save the transform of the player object

    void Start()
    {
        playerTransform = player.transform; // set playerTransform to the transform of the passed player GameObject
    }

    // Update after all other updates are run
    void LateUpdate()
    {
        Vector3 temp = transform.position;      // get current camera position
        temp.x = playerTransform.position.x;    // get current character y position
        temp.y = playerTransform.position.y;    // get current character y position
        transform.position = temp;              // set new camera position
    }
}
