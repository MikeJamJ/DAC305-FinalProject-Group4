using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelEndScreen : MonoBehaviour
{
    public Text completionTime;
    public Text averageStamina;
    public Text emptiedStamina;
    public Text averageDistance;

    public void Setup(float time, float stamina, float empty, float distance) {
        gameObject.SetActive(true);
        completionTime.text = time.ToString() + " s";
        averageStamina.text = stamina.ToString() + " %";
        emptiedStamina.text = empty.ToString() + " times";
        averageDistance.text = distance.ToString() + " m";
    }

    public void RestartButton() {
        
    }
}
