using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    
    public TMP_Text scoreText;


    void Awake()
    {
        scoreText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        scoreText.text = "Score: " + GameManager.instance.GetScore() + "\nHighscore: " + GameManager.instance.GetHighScore();
    }
}
