using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public LevelCompleteScreen levelCompleteScreen;
    public GameOverScreen gameOverScreen;
    public Character2Dcontroller charCont;

    public static GameManager instance;
    
    // Used for timing how long it takes to complete a level
    private float levelStartTime;
    private float levelEndTime;

    // Used for checking if the game is paused
    public static bool gameIsPaused;

    void Awake() {
        instance = this;
        Time.timeScale = 1f;
        gameIsPaused = false;
    }

    // Start is called before the first frame update
    void Start()
    {;
        levelCompleteScreen = levelCompleteScreen.GetComponent<LevelCompleteScreen>();
        gameOverScreen = gameOverScreen.GetComponent<GameOverScreen>();
        charCont = charCont.GetComponent<Character2Dcontroller>();
        levelStartTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (gameIsPaused) {
            Time.timeScale = 0f;
        } else {
            Time.timeScale = 1f;
        }
    }

    // Function for pausing the game
    private void Pause() {
        gameIsPaused = true;
        Time.timeScale = 0f;
    }

    // Function for handling when a level is complete
    public void LevelComplete()
    {   
        Pause();
        levelEndTime = Time.time;
        levelCompleteScreen.Setup(
            levelEndTime - levelStartTime,
            StaminaBar.instance.getAverage(),
            StaminaBar.instance.getTimesEmptied(),
            charCont.getAverage());
    }

    // Function for handling when a game is over
    public void GameOver()
    {
        Pause();
        gameOverScreen.Setup();        
    }

    // Function for restarting the level
    public void Restart() {
        Debug.Log(Time.timeScale);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
