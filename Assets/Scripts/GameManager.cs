using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, IDataPersistence
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

    public int highscore;
    public int score;

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
        LoadData(DataPersistenceManager.instance.gameData);
        ChangeGameState(GameState.Menu);
    }

    public void LoadData(GameData data)
    {
        this.highscore = data.highscore;
    }
    public void SaveData(ref GameData data)
    {
        data.highscore = highscore;
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
                highScoreText.transform.SetParent(playPanel.transform);
                ScoreManager.instance.scoreText.transform.SetParent(playPanel.transform);
                gameTimerText.transform.SetParent(playPanel.transform);
                menuPanel.SetActive(false);
                gameBoard.SetActive(true);
                musicPlayer.Play();
                playPanel.SetActive(true);
                gameOverPanel.SetActive(false);
                gameTimer = 90f; // Reset the timer when the game starts
                highScoreText.text = "High Score: " + highscore;
                break;
            case GameState.GameOver:
                musicPlayer.Stop();
                highScoreText.transform.SetParent(gameOverPanel.transform);
                gameTimerText.transform.SetParent(gameOverPanel.transform);
                ScoreManager.instance.scoreText.transform.SetParent(gameOverPanel.transform);
                menuPanel.SetActive(false);
                gameBoard.SetActive(false);
                playPanel.SetActive(false);
                gameOverPanel.SetActive(true);
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
        score = ScoreManager.instance.score;

        if (score > highscore)
        {
            highscore = score;
            highScoreText.text = "New High Score! " + highscore;
        }
        else
        {
            highScoreText.text = "Score: " + highscore;
        }

        ScoreManager.instance.scoreText.text = "Your score: " + score;
        gameTimer = 90f;
        ChangeGameState(GameState.GameOver);
    }

    private void DisplayHighScore()
    {
        highScoreText.text += "\nHigh Score: " + highscore;
    }
}