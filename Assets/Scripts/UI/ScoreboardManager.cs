using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    // Game UI
    [SerializeField] private Image background;
    [SerializeField] private List<GameObject> pointObjects;
    [SerializeField] private GameObject matchInfo;


    private void OnEnable()
    {
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange += OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnWinnerDetermined += OfflineGameStateManager_OnWinnerDetermined;
        OfflineGameStateManager.OnPointChange += OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange -= OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnWinnerDetermined -= OfflineGameStateManager_OnWinnerDetermined;
        OfflineGameStateManager.OnPointChange -= OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.NotPlaying)
        {
            //SetElementText(scoreText, "");
            //SetElementText(matchText, "");
            //SetElementText(winnerText, "");
        }
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        //SetElementText(scoreText, "0 - 0");
    }

    private void OfflineGameStateManager_OnMatchTextChange(string text)
    {
        //SetElementText(matchText, text);

    }

    private void OfflineGameStateManager_OnWinnerDetermined(int playerNum)
    {
        if (playerNum == 0)
        {
            //SetElementText(winnerText, "");
        }
        else
        {
            //SetElementText(winnerText, "Player " + playerNum.ToString() + " wins!");
        }

    }

    private void OfflineGameStateManager_OnPointChange(int playerOneScore, int playerTwoScore)
    {
        //SetElementText(scoreText, playerOneScore + " - " + playerTwoScore);
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
