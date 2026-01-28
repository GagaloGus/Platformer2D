using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager Instance;

    public enum GameState { Menu, Playing, Paused, GameOver}
    public GameState currentState;

    private int score = 0;
    //private int playerLives = 3;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;

        } else
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        ChangeState(GameState.Menu);
    }

    public void ChangeState(GameState newState)
    {
        currentState = newState;
    }

    public void StartGame()
    {
        score = 0;
        ChangeState(GameState.Playing);
        
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        ChangeState(GameState.Paused);
    }

    public void ResumeGame()
    {
        Time.timeScale = 0f;
        ChangeState(GameState.Playing);
    }

    public void AddScore (int amount)
    {
        score += amount;
    }

    /* public void PlayerDie()
     {
         playerLives--;

         if (playerLives <= 0)
         {
             ChangeState(GameState.GameOver);
         }
     }*/

    public int GetScore() => score;
    //public int GetLives() => playerLives;

}
