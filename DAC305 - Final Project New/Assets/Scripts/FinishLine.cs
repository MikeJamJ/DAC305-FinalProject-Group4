using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinishLine : MonoBehaviour
{       
    void OnTriggerEnter2D(Collider2D collision)
    {   
        // If the player has touched the finihs line, enter level complete
        if(collision.tag == "Player")
        {
            GameManager.instance.LevelComplete();
        }
    }
}
