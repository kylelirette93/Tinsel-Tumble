using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    // Create an instance of score manager.
    public static ScoreManager instance;

    int score;
    public TextMeshProUGUI scoreText;

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
    void Start()
    {
        // Set the score to 0 at the start.
        score = 0;
    }

    public void AddScore(int matchedCells)
    {
        // Calculate the score based on the number of matched cells.
        score += matchedCells * 25;
        UpdateScoreText();
    }

    void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;   
    }
}
