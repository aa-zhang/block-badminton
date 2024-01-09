using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public int playerOneScore = 0;
    public int playerTwoScore = 0;

    [SerializeField] private int winningScore = 21;

    // Start is called before the first frame update
    void Start()
    {
        menu = canvas.GetComponent<GameMenu>();
        menu.ShowMenu(false);
        winnerText.gameObject.SetActive(false);
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
            winnerText.text = "The strongest badminton player in history wins!";
            menu.ShowMenu(true);
            winnerText.gameObject.SetActive(true);
        }
        else if (playerTwoScore >= winningScore)
        {
            winnerText.text = "The strongest badminton player of today wins!";
            menu.ShowMenu(true);
            winnerText.gameObject.SetActive(true);
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
