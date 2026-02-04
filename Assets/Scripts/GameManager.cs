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
    int coins;

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

    public int GetScore()
    {
        return score;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public void Victoria()
    {
        gameState = GameState.Victory;
        Time.timeScale = 0f;
    }

    public void GetCoins(int amount)
    {
        coins += amount;
    }

    public void CreateExplosion(Transform objTransform)
    {
        CreateExplosion(objTransform.position, objTransform.localScale);
    }

    public void CreateExplosion(Vector3 position, Vector3 localScale)
    {
        Transform kaput = Instantiate(Prefab_Explosion).transform;
        kaput.position = position;
        kaput.localScale = localScale;
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeAsyncScene(sceneName));
    }

    IEnumerator ChangeAsyncScene(string sceneName)
    {
        print($"loading scene async {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
