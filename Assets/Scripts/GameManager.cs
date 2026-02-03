using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { Playing, Paused, Victory, GameOver }

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject Prefab_Explosion;

    [Header("Game State")]
    public GameState gameState = GameState.Playing;

    [Header("Score")]
    private int score;
    private int highScore;

    [Header("Lives")]
    private int maxLives = 3;
    private int currentLives;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        } else
        {
            Destroy(gameObject);
            return;
        }
       
        highScore = PlayerPrefs.GetInt("HighScore", 0);

    }

    void Start()
    {
        Time.timeScale = 1f;
        score = 0;
        gameState = GameState.Playing;

        AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level1_song);
    }

    public void CreateExplosion(Transform objTransform)
    {
        Transform kaput = Instantiate(Prefab_Explosion).transform;
        kaput.position = objTransform.position;
        kaput.localScale = objTransform.localScale;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        print($"loaded scene {sceneName}");
    }

    public void AddScore(int points)
    {
        score += points;

        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
    }

    public void LoseLife()
    {
        if(gameState != GameState.Playing) return;

        currentLives --;


        if (currentLives <= 0)
        {
            GameOver();
        }
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

    void GameOver()
    {
        gameState = GameState.GameOver;
        Time.timeScale = 0f;

        // lo que se debería cargar cuando se acabe
    }

    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public int GetLives()
    {
        return currentLives;
    }

    public void Victoria()
    {
        gameState = GameState.Victory;
        Time.timeScale = 0f;
    }

}
