using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class ScoreManager : MonoBehaviour
{
    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;
    public TextMeshProUGUI readyCountText;

    public int playerOneScore = 0;
    public int playerTwoScore = 0;

    private bool hasGameStarted = false;

    [SerializeField] private int winningScore = 21; // Default winning score
    [SerializeField] private int maxScore = 30; // Deuce score cap


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
        if (!hasGameStarted)
        {
            ShowReadyCount();
        }
        else
        {
            CheckScore();
            DisplayScore();
        }
    }

    private void ShowReadyCount()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        readyCountText.text = $"{playerCount}/2 Players joined";

        if (playerCount >= 2)
        {
            InitiateMatch();
            hasGameStarted = true;
        }
    }

    private void InitiateMatch()
    {
        readyCountText.gameObject.SetActive(false);
        // Spawn birdie
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore, playerTwoScore);
        int losingPlayerScore = Mathf.Min(playerOneScore, playerTwoScore);

        // Check if the match is in a Duece or Match Point state
        if (winningPlayerScore >= winningScore - 1)
        {
            if (winningPlayerScore == losingPlayerScore)
            {
                matchText.text = "Deuce";
            }
            else
            {
                matchText.text = "Match Point";
            } 
        }


        // Check if a player has won
        if ((winningPlayerScore >= winningScore && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == maxScore)
        {
            matchText.text = "Match Over!";

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
