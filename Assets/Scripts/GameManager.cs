using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject menuPanel;

    // Play state properties.
    public GameObject gameBoard;
    public GameObject playPanel;
    public AudioSource musicPlayer;
    public TextMeshProUGUI gameTimerText;
    public float gameTimer;

    public GameObject gameOverPanel;
    public TextMeshProUGUI highScoreText;

    public enum GameState
    {
        Menu,
        Play,
        GameOver
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeGameState(GameState.Menu);
    }

    private void Update()
    {
        if (currentState == GameState.Play)
        {
            gameTimer -= Time.deltaTime;
            gameTimerText.text = "Time Left: " + gameTimer.ToString("F2");

            if (gameTimer <= 0)
            {
                gameTimer = 0;
                gameTimerText.text = "Out of Time!";
                EndGame();
            }
        }
    }

    private GameState currentState;

    public void ChangeGameState(GameState gameState)
    {
        currentState = gameState;

        switch (gameState)
        {
            case GameState.Menu:
                musicPlayer.Stop();
                menuPanel.SetActive(true);
                gameBoard.SetActive(false);
                playPanel.SetActive(false);
                gameOverPanel.SetActive(false);
                break;
            case GameState.Play:
                menuPanel.SetActive(false);
                highScoreText.transform.SetParent(playPanel.transform);
                gameTimerText.transform.SetParent(playPanel.transform);
                gameBoard.SetActive(true);
                musicPlayer.Play();
                playPanel.SetActive(true);
                gameOverPanel.SetActive(false);
                gameTimer = 90f; // Reset the timer when the game starts
                break;
            case GameState.GameOver:
                musicPlayer.Stop();
                highScoreText.transform.SetParent(gameOverPanel.transform);
                gameTimerText.transform.SetParent(gameOverPanel.transform);
                menuPanel.SetActive(false);
                gameBoard.SetActive(false);
                playPanel.SetActive(false);
                gameOverPanel.SetActive(true);
                DisplayHighScore();
                break;
        }
    }

    public void StartGame()
    {
        ChangeGameState(GameState.Play);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private void EndGame()
    {
        int currentScore = ScoreManager.instance.score;
        int highScore = PlayerPrefs.GetInt("HighScore", 0);

        if (currentScore > highScore)
        {
            PlayerPrefs.SetInt("HighScore", currentScore);
        }

        ScoreManager.instance.score = 0;
        ScoreManager.instance.UpdateScoreText();
        gameTimer = 90f;
        ChangeGameState(GameState.GameOver);
    }

    private void DisplayHighScore()
    {
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        highScoreText.text = "High Score: " + highScore;
    }
}