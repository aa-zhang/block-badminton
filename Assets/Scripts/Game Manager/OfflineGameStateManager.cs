using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public enum GameState { NotPlaying, Serving, Playing, GameOver }

public class OfflineGameStateManager : MonoBehaviour
{
    // Score variables
    private int playerOneScore = 0;
    private int playerTwoScore = 0;
    private int servingPlayerNum = 0;

    private GameState gameState = GameState.NotPlaying;
    private PlayMode playMode;

    [SerializeField] private GameObject birdiePrefab;

    // Events
    public static Action<GameState> OnGameStateChange;
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
        GameModeManager.OnGameStart += GameModeManager_OnGameStart;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
        OfflineServeController.OnHitServe += OfflineServeController_OnHitServe;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        GameModeManager.OnGameStart -= GameModeManager_OnGameStart;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
        OfflineServeController.OnHitServe -= OfflineServeController_OnHitServe;
    }

    private void InitiateGameRpc()
    {
        gameState = GameState.Playing;
        OnGameStateChange?.Invoke(gameState);
        //OnStartMatch();
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
        gameState = GameState.Playing;
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

        if (gameState == GameState.Playing && !gameEnv.isTraining)
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
                OnMatchTextChange?.Invoke("Deuce");
            }
            else
            {
                OnMatchTextChange?.Invoke("Match Point");
            }
        }

        // Check if a player has won
        if ((winningPlayerScore >= Constants.WINNING_SCORE && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.MAX_SCORE)
        {
            OnMatchTextChange?.Invoke("Match Over!");
            gameState = GameState.GameOver;
            OnGameStateChange?.Invoke(gameState);

            if (playerOneScore >= Constants.WINNING_SCORE)
            {
                OnWinnerDetermined?.Invoke(1);
            }
            else if (playerTwoScore >= Constants.WINNING_SCORE)
            {
                OnWinnerDetermined?.Invoke(2);
            }
        }
    }
    

    private void GameMenu_OnGameRestart(int gameEnvId)
    {
        if (gameEnv.id == gameEnvId)
        {
            RestartGameRpc();
        }
    }

    private void GameModeManager_OnGameStart(PlayMode playMode)
    {
        this.playMode = playMode;
        RestartGameRpc();
    }

    public void RestartGameRpc()
    {
        OnMatchTextChange?.Invoke("");
        OnWinnerDetermined?.Invoke(0);
        OnPointChange?.Invoke(0, 0);

        // Reset score values
        playerOneScore = 0;
        playerTwoScore = 0;

        InitiateGameRpc();
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
