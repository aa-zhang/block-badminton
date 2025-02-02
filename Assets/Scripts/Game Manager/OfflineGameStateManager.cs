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

    private bool gameInProgress = false;

    [SerializeField] private GameObject birdiePrefab;

    // Events
    public delegate void BirdieObjectHandler(GameObject birdie, int gameEnvId);
    public static BirdieObjectHandler OnBirdieInitialized;

    public delegate void StartMatchHandler();
    public static StartMatchHandler OnStartMatch;

    public delegate void ServeHandler(int playerNum, int gameEnvId);
    public static ServeHandler OnBeginServe;

    private GameEnvironmentManager gameEnv;


    void Start()
    {
        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
    }

    private void FixedUpdate()
    {
        DisplayScore();
    }

    private void OnEnable()
    {
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        GameMenu.OnGameStart += GameMenu_OnGameStart;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        GameMenu.OnGameStart -= GameMenu_OnGameStart;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
    }

    private void InitiateGameRpc()
    {
        gameInProgress = true;
        //OnStartMatch();
    }

    private void SpawnBirdie()
    {
        // Initialize birdie for clients
        OnBirdieInitialized?.Invoke(birdiePrefab, gameEnv.id);

        if (!gameEnv.isTraining)
        {
            SelectRandomServer();
        }
    }


    private void SelectRandomServer()
    {
        // Select random number from {1, 2}
        servingPlayerNum = Random.Range(1, 3);
        BeginServeRpc();
    }


    private void BeginServeRpc()
    {
        OnBeginServe(servingPlayerNum, gameEnv.id);
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

        if (gameInProgress && !gameEnv.isTraining)
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
        if (winningPlayerScore >= Constants.WINNING_SCORE - 1)
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
        if ((winningPlayerScore >= Constants.WINNING_SCORE && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.MAX_SCORE)
        {
            SetMatchTextRpc("Match Over!");
            gameInProgress = false;

            if (playerOneScore >= Constants.WINNING_SCORE)
            {
                SetWinnerTextRpc("Player 1 wins!");
                //ShowMenuRpc(true);
            }
            else if (playerTwoScore >= Constants.WINNING_SCORE)
            {
                SetWinnerTextRpc("Player 2 wins!");
                //ShowMenuRpc(true);
            }
        }
    }

    //private void ShowMenuRpc(bool show)
    //{
    //    GameMenu.Instance.ShowMenu(show);
    //}

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

    private void GameMenu_OnGameRestart(int gameEnvId)
    {
        if (gameEnv.id == gameEnvId)
        {
            RestartGameRpc();
        }
    }

    private void GameMenu_OnGameStart()
    {
        SpawnBirdie();
        RestartGameRpc();
    }

    public void RestartGameRpc()
    {
        // Hide menu and other UI
        //ShowMenuRpc(false);
        SetMatchTextRpc("");
        SetWinnerTextRpc("");

        // Reset score values
        playerOneScore = 0;
        playerTwoScore = 0;

        InitiateGameRpc();
        SelectRandomServer();
    }
}
