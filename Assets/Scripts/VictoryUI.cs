using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public TMP_Text finalScoreText;

    void Start()
    {
        finalScoreText.text = "Score: " + GameManager.instance.GetScore() + "\nHighscore: " + GameManager.instance.GetHighScore();
    }
}
