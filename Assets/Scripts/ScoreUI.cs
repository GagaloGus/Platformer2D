using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    
    public TMP_Text scoreText;
    public TMP_Text highScoreText;

    void Update()
    {
        scoreText.text = "Score: " + GameManager.Instance.GetScore();
        highScoreText.text = "HighScore: " + GameManager.Instance.GetHighScore();
    }
}
