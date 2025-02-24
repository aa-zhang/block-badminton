using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScoreboardManager : MonoBehaviour
{
    // Game UI
    private CanvasGroup canvasGroup;

    [SerializeField] private List<Image> matchWinnerSquares = new List<Image>();
    [SerializeField] private TextMeshProUGUI matchInfo;
    [SerializeField] private TextMeshProUGUI playerOneScore;
    [SerializeField] private TextMeshProUGUI playerTwoScore;

    private Color winnerSquareEmptyColor = new Color(0f, 0f, 0f, 125f / 255f);
    private Color winnerSquareRedColor = new Color(1f, 0f, 0f, 200f / 255f);
    private Color winnerSquareBlueColor = new Color(0f, 0f, 1f, 200f / 255f);


    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
    }


    private void OnEnable()
    {
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange += OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange += OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange -= OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange -= OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.Rallying || newGameState == GameState.NotPlaying)
        {
            canvasGroup.DOFade(0f, 0.25f);
        }
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        canvasGroup.DOFade(1f, 0.25f);
    }

    private void OfflineGameStateManager_OnMatchTextChange(string text)
    {
        SetElementText(matchInfo, text);
    }

    private void OfflineGameStateManager_OnMatchNumChange(int matchNum, int winner)
    {
        // color/reset the match winner squares
        if (matchNum == 0)
        {
            ResetMatchWinnerSquares();
        }
        else
        {
            Color winnerColor = winner == 1 ? winnerSquareRedColor : winnerSquareBlueColor;
            matchWinnerSquares[matchNum - 1].color = winnerColor;
        }

    }

    private void OfflineGameStateManager_OnPointChange(int playerOneScore, int playerTwoScore)
    {
        SetElementText(this.playerOneScore, playerOneScore.ToString());
        SetElementText(this.playerTwoScore, playerTwoScore.ToString());
        canvasGroup.DOFade(1f, 0.25f);
    }


    private void SetElementText(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }

    private void ResetMatchWinnerSquares()
    {
        foreach (Image square in matchWinnerSquares)
        {
            square.color = winnerSquareEmptyColor;

}
    }
}
