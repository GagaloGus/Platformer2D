using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;


    [Header("Game State")]
    public GameState gameState = GameState.Playing;

    [Header("Score")]
    private int score;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        } else
        {
            Destroy(gameObject);
        }
       
    }

    void Start()
    {
        Time.timeScale = 1f;
        score = 0;
        gameState = GameState.Playing;
    }

    public void AddScore(int points)
    {
        score += points;
    }

    public void PauseGame()
    {
        if (gameState != GameState.Playing) return;

        gameState = GameState.Paused;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (gameState != GameState.Paused) return;

        gameState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Victoria()
    {
        gameState = GameState.Victory;
        Time.timeScale = 0f;
    }

}
