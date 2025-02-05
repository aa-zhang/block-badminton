using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class UIManager : MonoBehaviour
{
    // Game UI
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI matchText;


    private void OnEnable()
    {
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange += OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnWinnerTextChange += OfflineGameStateManager_OnWinnerTextChange;
        OfflineGameStateManager.OnPointChange += OfflineGameStateManager_OnPointChange;
        GameMenu.OnGameStart += GameMenu_OnGameStart;
        GameMenu.OnReturnToTitleScreen += GameMenu_OnReturnToTitleScreen;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange -= OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnWinnerTextChange -= OfflineGameStateManager_OnWinnerTextChange;
        OfflineGameStateManager.OnPointChange -= OfflineGameStateManager_OnPointChange;
        GameMenu.OnGameStart -= GameMenu_OnGameStart;
        GameMenu.OnReturnToTitleScreen -= GameMenu_OnReturnToTitleScreen;

    }

    private void OfflineGameStateManager_OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.Playing)
        {
        }
    }

    private void GameMenu_OnGameStart()
    {
        SetElementText(scoreText, "0 - 0");
    }

    private void GameMenu_OnReturnToTitleScreen()
    {
        SetElementText(scoreText, "");
    }

    private void OfflineGameStateManager_OnMatchTextChange(string text)
    {
        SetElementText(matchText, text);

    }

    private void OfflineGameStateManager_OnWinnerTextChange(string text)
    {
        SetElementText(winnerText, text);

    }

    private void OfflineGameStateManager_OnPointChange(int playerOneScore, int playerTwoScore)
    {
        SetElementText(scoreText, playerOneScore + " - " + playerTwoScore);
    }


    private void SetElementText(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }


    //private void CreateAndJoinRelay_OnRelayJoined(bool isHost, string joinCode)
    //{
    //    if (isHost)
    //    {
    //        roomCodeText.text = joinCode;
    //    }
    //    createRelayButton.gameObject.SetActive(false);
    //    joinRelayButton.gameObject.SetActive(false);
    //    roomCodeInput.gameObject.SetActive(false);
    //    backButton.gameObject.SetActive(false);
    //}

    //private void GameStateManager_OnStartMatch()
    //{
    //    roomCodeText.gameObject.SetActive(false);
    //    scoreText.gameObject.SetActive(true);
    //    matchText.gameObject.SetActive(true);
    //    winnerText.gameObject.SetActive(true);
    //}



}
