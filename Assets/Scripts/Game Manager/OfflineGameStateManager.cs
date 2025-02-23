using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum GameState { NotPlaying, Serving, Rallying, GameOver }

public class OfflineGameStateManager : MonoBehaviour
{
    // Score variables
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    private int playerOneMatchesWon = 0;
    private int playerTwoMatchesWon = 0;
    private int servingPlayerNum = 0;

    private GameState gameState = GameState.NotPlaying;
    private PlayMode playMode;

    [SerializeField] private GameObject birdiePrefab;

    // Events
    public static Action<GameState> OnGameStateChange;
    public static Action<int> OnMatchNumChange;
    public static Action<string> OnMatchTextChange;
    public static Action<int> OnWinnerDetermined;
    public static Action<int, int> OnPointChange;


    public delegate void StartMatchHandler();
    public static StartMatchHandler OnStartMatch;

    public delegate void ServeHandler(int playerNum, int gameEnvId);
    public static ServeHandler OnBeginServe;

    private GameEnvironmentManager gameEnv;


    void Start()
    {
        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
    }

    private void OnEnable()
    {
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
        OfflineServeController.OnHitServe += OfflineServeController_OnHitServe;
        GameMenu.OnReturnToTitleScreen += GameMenu_OnReturnToTitleScreen;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
        OfflineServeController.OnHitServe -= OfflineServeController_OnHitServe;
        GameMenu.OnReturnToTitleScreen -= GameMenu_OnReturnToTitleScreen;
    }

    private void SelectRandomServer()
    {
        // Select random number from {1, 2}
        servingPlayerNum = UnityEngine.Random.Range(1, 3);
        BeginServeRpc();
    }

    private void SelectServer(int playerNum)
    {
        // Server is player 1 or 2
        servingPlayerNum = playerNum;
        BeginServeRpc();
    }


    private void BeginServeRpc()
    {
        gameState = GameState.Serving;
        OnBeginServe(servingPlayerNum, gameEnv.id);
        OnGameStateChange?.Invoke(gameState);
    }

    private void OfflineServeController_OnHitServe()
    {
        gameState = GameState.Rallying;
        OnGameStateChange?.Invoke(gameState);
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

        OnPointChange?.Invoke(playerOneScore, playerTwoScore);

        servingPlayerNum = scoringPlayerNum;

        CheckScore(); // check if game has ended

        if (gameState == GameState.Rallying && !gameEnv.isTraining)
        {
            // Start next serve after a 1 second delay
            Invoke("BeginServeRpc", 1);
        }
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore, playerTwoScore);
        int losingPlayerScore = Mathf.Min(playerOneScore, playerTwoScore);

        if (winningPlayerScore >= Constants.WINNING_SCORE - 1)
        {
            // Check if the match is in a Deuce or Match Point state
            if (winningPlayerScore == losingPlayerScore) OnMatchTextChange?.Invoke("Deuce");
            else OnMatchTextChange?.Invoke("Match Point");
        }

        // Check if a player has won
        if ((winningPlayerScore >= Constants.WINNING_SCORE && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.MAX_SCORE)
        {
            int winner = playerOneScore >= Constants.WINNING_SCORE ? 1 : 2;

            if (winner == 1) playerOneMatchesWon++;
            else playerTwoMatchesWon++;


            if (playerOneMatchesWon >= 2 || playerTwoMatchesWon >= 2)
            {
                gameState = GameState.GameOver;
                OnGameStateChange?.Invoke(gameState);
                OnMatchTextChange?.Invoke("Player " + winner + " wins!");
            }
            else
            {
                OnMatchNumChange?.Invoke(playerOneMatchesWon + playerTwoMatchesWon);
                ResetGameValues(false);
            }

        }
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        this.playMode = playMode;
        RestartGameRpc();
    }

    private void GameMenu_OnReturnToTitleScreen()
    {
        gameState = GameState.NotPlaying;
        OnGameStateChange?.Invoke(gameState);
    }

    private void ResetGameValues(bool resetMatches)
    {
        if (resetMatches)
        {
            OnMatchNumChange?.Invoke(0);
            playerOneMatchesWon = 0;
            playerTwoMatchesWon = 0;
        }

        OnMatchTextChange?.Invoke("");
        OnWinnerDetermined?.Invoke(0);
        OnPointChange?.Invoke(0, 0);

        // Reset score values
        playerOneScore = 0;
        playerTwoScore = 0;
    }

    public void RestartGameRpc()
    {
        ResetGameValues(true);


        if (playMode == PlayMode.Human)
        {
            SelectRandomServer();
        }
        else if (playMode == PlayMode.AI)
        {
            // Always let player serve first against AI
            SelectServer(1);
        }
    }
}
