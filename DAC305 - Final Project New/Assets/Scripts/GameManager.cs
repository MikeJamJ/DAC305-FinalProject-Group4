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

    private string[] sceneNames =  new string[] {
        "Level1",
        "Level2",
        "Level3"
    };

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
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadNextLevel() {
        string currentScene = SceneManager.GetActiveScene().name;

        switch (currentScene) {
            case "Level1":
                Debug.Log("2");
                SceneManager.LoadScene(sceneNames[1]);
                break;
            case "Level2":
                Debug.Log("3");
                SceneManager.LoadScene(sceneNames[2]);
                break;
            case "Level3":
                Debug.Log("4");
                SceneManager.LoadScene(sceneNames[0]);
                break;
            default:
                break;

        }
    }
}
