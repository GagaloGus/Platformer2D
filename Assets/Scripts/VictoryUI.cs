using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryUI : MonoBehaviour
{
    public TMP_Text finalScoreText;
    public TMP_Text highScoreText;

    void Start()
    {
        finalScoreText.text = "Score: " + GameManager.Instance.GetScore();
        highScoreText.text = "HighScore: " + GameManager.Instance.GetHighScore();
    }
}
