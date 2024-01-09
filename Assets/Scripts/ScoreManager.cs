using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI scoreText;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;

    [SerializeField] private int winningScore = 21;

    // Start is called before the first frame update
    void Start()
    {
        menu = canvas.GetComponent<GameMenu>();
        menu.ShowMenu(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        CheckScore();
        DisplayScore();
    }

    private void CheckScore()
    {
        if (playerOneScore >= winningScore)
        {
            menu.ShowMenu(true);
        }
        else if (playerTwoScore >= winningScore)
        {
            menu.ShowMenu(true);
        }
    }    

    private void DisplayScore()
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
