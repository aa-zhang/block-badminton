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

    [SerializeField] private RectTransform background;
    [SerializeField] private List<Image> pointSquares;
    [SerializeField] private RectTransform matchInfoBg;

    private List<TextMeshProUGUI> pointObjectTexts = new List<TextMeshProUGUI>();
    private TextMeshProUGUI matchInfoText;

    private int backgroundDefaultWidth = 350;
    private int backgroundExtendWidth = 85; // extend background by this amount after each match
    private int matchNum = 0;


    void Start()
    {
        canvasGroup = gameObject.GetComponent<CanvasGroup>();
        matchInfoText = matchInfoBg.GetComponentInChildren<TextMeshProUGUI>();
        for (int i = 0; i < pointSquares.Count; i++)
        {
            pointObjectTexts.Add(pointSquares[i].GetComponentInChildren<TextMeshProUGUI>());
        }
    }


    private void OnEnable()
    {
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange += OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange += OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
    }

    private void OnDisable()
    {
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
        OfflineGameStateManager.OnMatchTextChange -= OfflineGameStateManager_OnMatchTextChange;
        OfflineGameStateManager.OnMatchNumChange += OfflineGameStateManager_OnMatchNumChange;
        OfflineGameStateManager.OnPointChange -= OfflineGameStateManager_OnPointChange;
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        canvasGroup.DOFade(1f, 0.5f);
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState newGameState)
    {
        if (newGameState == GameState.Playing || newGameState == GameState.NotPlaying)
        {
            canvasGroup.DOFade(0f, 0.5f);
        }
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        //SetElementText(scoreText, "0 - 0");
    }

    private void OfflineGameStateManager_OnMatchTextChange(string text)
    {
        if (text.Length == 0)
        {
            matchInfoBg.gameObject.SetActive(false);
        }
        else
        {
            matchInfoBg.gameObject.SetActive(true);
            SetElementText(matchInfoText, text);
        }
    }

    private void OfflineGameStateManager_OnMatchNumChange(int matchNum)
    {
        this.matchNum = matchNum;
        Vector2 newBackgroundWidth = background.sizeDelta;
        Vector2 newMatchInfoBgWidth = matchInfoBg.sizeDelta;


        if (matchNum == 0)
        {
            for (int i = 2; i < 6; i++)
            {
                pointSquares[i].gameObject.SetActive(false);
            }

            newBackgroundWidth.x = backgroundDefaultWidth;
            newMatchInfoBgWidth.x = backgroundDefaultWidth;
        }
        else
        {
            newBackgroundWidth.x += backgroundExtendWidth;
            newMatchInfoBgWidth.x += backgroundExtendWidth;

        }

        pointSquares[matchNum * 2].gameObject.SetActive(true);
        pointSquares[matchNum * 2 + 1].gameObject.SetActive(true);

        background.sizeDelta = newBackgroundWidth;
        matchInfoBg.sizeDelta = newMatchInfoBgWidth;

    }

    private void OfflineGameStateManager_OnPointChange(int playerOneScore, int playerTwoScore)
    {
        SetElementText(pointObjectTexts[matchNum * 2], playerOneScore.ToString());
        SetElementText(pointObjectTexts[matchNum * 2 + 1], playerTwoScore.ToString());
    }


    private void SetElementText(TextMeshProUGUI textElement, string text)
    {
        textElement.text = text;
    }
}
