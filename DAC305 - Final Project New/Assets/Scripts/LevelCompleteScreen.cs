using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelCompleteScreen : MonoBehaviour
{   
    // Text fields to update with information
    public Text completionTime;
    public Text averageStamina;
    public Text emptiedStamina;
    public Text averageDistance;

    public Button restartButton;
    public Button nextLevelButton;

    void Start() {
        restartButton.onClick.AddListener(RestartButton);
        nextLevelButton.onClick.AddListener(NextLevelButton);
    }

    // Function for assigning text field values when the level is completed
    public void Setup(float time, float stamina, float empty, float distance) {
        gameObject.SetActive(true);
        completionTime.text = time.ToString("F2") + " s";
        averageStamina.text = (stamina * 100).ToString("F2") + " %";
        emptiedStamina.text = empty.ToString() + " times";
        averageDistance.text = distance.ToString("F2") + " m";
    }

    // Function for handling the restart button
    public void RestartButton() {
        GameManager.instance.Restart();
    }

    // Function for handling the restart button
    public void NextLevelButton() {
        GameManager.instance.LoadNextLevel();
    }
}
