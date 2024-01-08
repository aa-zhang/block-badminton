using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = playerOneScore + " - " + playerTwoScore;
    }

    public void IncreaseScore(int playerNum)
    {
        if (playerNum == 1)
        {
            playerOneScore++;
        }
        else
        {
            playerTwoScore++;
        }
    }

    public void ResetScores()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
    }
}
