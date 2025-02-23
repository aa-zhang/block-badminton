using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreboardManager : MonoBehaviour
{
    // Game UI
    [SerializeField] private RectTransform background;
    [SerializeField] private List<GameObject> pointObjects;
    private List<TextMeshProUGUI> pointObjectTexts = new List<TextMeshProUGUI>();

    [SerializeField] private GameObject matchInfo;
    private TextMeshProUGUI matchInfoText;


    private int backgroundDefaultWidth = 350;
    private int backgroundExtendWidth = 85; // extend background by this amount after each match
    private int matchNum = 0;


    void Start()
    {
        matchInfoText = matchInfo.GetComponentInChildren<TextMeshProUGUI>();
        print(matchInfoText);
        SetElementText(matchInfoText, "test");
        for (int i = 0; i < pointObjects.Count; i++)
        {
            pointObjectTexts.Add(pointObjects[i].GetComponentInChildren<TextMeshProUGUI>());
        }
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
        if (text.Length == 0) matchInfo.SetActive(false);
        else SetElementText(matchInfoText, text);

    }

    private void OfflineGameStateManager_OnMatchNumChange(int matchNum)
    {
        this.matchNum = matchNum;
        Vector2 newBackgroundWidth = background.sizeDelta;

        if (matchNum == 0)
        {
            newBackgroundWidth.x = backgroundDefaultWidth;

            for (int i = 2; i < 6; i++)
            {
                pointObjects[i].SetActive(false);
            }
        }
        else
        {
            newBackgroundWidth.x += backgroundExtendWidth;
        }

        pointObjects[matchNum * 2].SetActive(true);
        pointObjects[matchNum * 2 + 1].SetActive(true);
        background.sizeDelta = newBackgroundWidth;



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
