using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class OfflineGameStateManager : MonoBehaviour
{
    // UI elements
    public GameObject canvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;

    // Score variables
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    private int servingPlayerNum = 0;

    private bool allPlayersJoined = false;
    private bool gameInProgress = false;


    // Network
    private List<ulong> clientIds = new List<ulong>();
    [SerializeField] private GameObject birdiePrefab;

    // Events
    public delegate void BirdieObjectHandler(GameObject birdie);
    public static BirdieObjectHandler OnBirdieInitialized;

    public delegate void StartMatchHandler();
    public static StartMatchHandler OnStartMatch;

    public delegate void ServeHandler(int playerNum);
    public static ServeHandler OnBeginServe;


    // Start is called before the first frame update
    void Start()
    {
        ShowMenuRpc(false);
    }

    private void FixedUpdate()
    {
        if (!allPlayersJoined)
        {
            WaitForAllPlayersToJoin();
        }
        else
        {
            DisplayScore();
        }
    }

    private void OnEnable()
    {
        BirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
    }

    private void OnDisable()
    {
        BirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
    }


    private void WaitForAllPlayersToJoin()
    {
        if (clientIds.Count >= 2)
        {
            InitiateGameRpc();
            SpawnBirdie();
        }
    }

    private void InitiateGameRpc()
    {
        allPlayersJoined = true;
        gameInProgress = true;
        OnStartMatch();
        Invoke("SelectRandomServer", 1);

    }

    private void SpawnBirdie()
    {
        Debug.Log("Game starting!");
        Debug.Log("Creating birdie");
        // Initialize birdie for clients
        OnBirdieInitialized(birdiePrefab);
        Invoke("SelectRandomServer", 1);
    }


    private void SelectRandomServer()
    {
        // Select random number from {1, 2}
        servingPlayerNum = Random.Range(1, 3);
        BeginServeRpc();
    }


    private void BeginServeRpc()
    {
        Debug.Log("serving player num: " + servingPlayerNum);
        OnBeginServe(servingPlayerNum);
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (scoringPlayerNum == 1)
        {
            playerOneScore++;
        }
        else
        {
            playerTwoScore++;
        }

        servingPlayerNum = scoringPlayerNum;

        CheckScore(); // sets gameInProgress to false if game has ended

        if (gameInProgress)
        {
            // Start next serve after a 1 second delay
            Invoke("BeginServeRpc", 1);
        }
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore, playerTwoScore);
        int losingPlayerScore = Mathf.Min(playerOneScore, playerTwoScore);

        // Check if the match is in a Deuce or Match Point state
        if (winningPlayerScore >= Constants.winningScore - 1)
        {
            if (winningPlayerScore == losingPlayerScore)
            {
                SetMatchTextRpc("Deuce");
            }
            else
            {
                SetMatchTextRpc("Match Point");
            }
        }

        // Check if a player has won
        if ((winningPlayerScore >= Constants.winningScore && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.maxScore)
        {
            SetMatchTextRpc("Match Over!");
            gameInProgress = false;

            if (playerOneScore >= Constants.winningScore)
            {
                SetWinnerTextRpc("The strongest badminton player in history wins!");
                ShowMenuRpc(true);
            }
            else if (playerTwoScore >= Constants.winningScore)
            {
                SetWinnerTextRpc("The strongest badminton player of today wins!");
                ShowMenuRpc(true);
            }
        }
    }

    private void ShowMenuRpc(bool show)
    {
        GameMenu.Instance.ShowMenu(show);

    }

    private void SetMatchTextRpc(string text)
    {
        matchText.text = text;

    }

    private void SetWinnerTextRpc(string text)
    {
        winnerText.text = text;
    }

    private void DisplayScore()
    {
        scoreText.text = playerOneScore + " - " + playerTwoScore;
    }

    private void GameMenu_OnGameRestart()
    {
        RestartGameRpc();
    }

    private void RestartGameRpc()
    {
        // Reset text and score values
        SetMatchTextRpc("");
        SetWinnerTextRpc("");
        ShowMenuRpc(false);
        playerOneScore = 0;
        playerTwoScore = 0;
        InitiateGameRpc();
        Invoke("SelectRandomServer", 1);
    }
}
