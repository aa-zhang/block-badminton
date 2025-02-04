using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using DG.Tweening;



public class GameMenu : MonoBehaviour
{
    public enum MenuType { TitleScreen, GameModeSelection, InGame, Settings, None }

    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }

    public static Action OnGameStart;
    public static Action<int> OnGameRestart;
    public static Action OnReturnToTitleScreen;

    [SerializeField] private MenuType menuState = MenuType.TitleScreen;
    [SerializeField] private float menuOpenXPos = -70f;
    [SerializeField] private float menuCloseXPos = 1150f;
    [SerializeField] private float menuLerpDuration = 0.25f;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject version;
    [SerializeField] private GameObject playText;
    [SerializeField] private GameObject settingsText;
    [SerializeField] private GameObject optionsText;

    [SerializeField] private List<Button> titleButtonList = new List<Button>();
    [SerializeField] private List<Button> gameModeButtonList = new List<Button>();
    [SerializeField] private List<Button> inGameButtonList = new List<Button>();
    [SerializeField] private List<Button> settingsButtonList = new List<Button>();

    [SerializeField] private float minButtonXPos = -240f;
    [SerializeField] private float buttonXDiff = 100f;
    [SerializeField] private float buttonYDiff = 140f;
    [SerializeField] private float delayBetweenButtons = 0.2f;
    [SerializeField] private float delayAfterSplashScreen = 1f;

    private GameState gameState = GameState.TitleScreen;

    private RectTransform menuRect;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        menuRect = GetComponent<RectTransform>();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
        StartCoroutine(WaitForSplashScreenToFinish());
    }

    private IEnumerator WaitForSplashScreenToFinish()
    {
        while (!SplashScreen.isFinished)
        {
            yield return null;
        }
        // Wait for an additional delay after the splash screen finishes
        yield return new WaitForSeconds(delayAfterSplashScreen);
        SequentiallyLoadButtons(titleButtonList);
    }

    public void PlayOfflineGame()
    {
        OnGameStart?.Invoke();
        gameState = GameState.Playing;
        ToggleMenu();
    }

    public void PlayAIGame()
    {
        SceneManager.LoadScene(2);
    }

    public void PlayOnlineGame()
    {
        SceneManager.LoadScene(3);
    }

    public void ReturnToStartScreen()
    {
        OnGameRestart?.Invoke(0);
        OnReturnToTitleScreen?.Invoke();
        gameState = GameState.TitleScreen;
        ShowTitleScreenMenu();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGameValues(int trainingEnvId)
    {
        OnGameRestart?.Invoke(trainingEnvId);
        ToggleMenu();
    }

    private void SetTitleAndVersionVisibility(bool isVisible)
    {
        title.SetActive(isVisible);
        version.SetActive(isVisible);
    }

    public void ShowTitleScreenMenu()
    {
        ResetButtonPositions();
        title.SetActive(true);
        version.SetActive(true);
        playText.SetActive(false);
        settingsText.SetActive(false);
        optionsText.SetActive(false);
        SequentiallyLoadButtons(titleButtonList);
        menuState = MenuType.TitleScreen;
    }

    public void ShowGameModesMenu()
    {
        ResetButtonPositions();
        title.SetActive(false);
        version.SetActive(false);
        playText.SetActive(true);
        settingsText.SetActive(false);
        optionsText.SetActive(false);
        SequentiallyLoadButtons(gameModeButtonList);
        menuState = MenuType.GameModeSelection;
    }

    public void ShowInGameMenu()
    {
        ResetButtonPositions();
        title.SetActive(false);
        version.SetActive(false);
        playText.SetActive(false);
        settingsText.SetActive(false);
        optionsText.SetActive(true);
        SequentiallyLoadButtons(inGameButtonList);
        menuState = MenuType.InGame;
    }

    public void ShowSettingsMenu()
    {
        ResetButtonPositions();
        title.SetActive(false);
        version.SetActive(false);
        playText.SetActive(false);
        settingsText.SetActive(true);
        optionsText.SetActive(false);
        SequentiallyLoadButtons(settingsButtonList);
        menuState = MenuType.Settings;
    }

    public void ShowPreviousMenu()
    {
        // When the Back button is pressed
        if (gameState == GameState.Playing || gameState == GameState.GameOver)
        {
            ShowInGameMenu();
        }
        else
        {
            ShowTitleScreenMenu();
        }
    }

    public void ShowMenu()
    {
        menuRect.DOKill(); // Stop any existing tween
        menuRect.DOAnchorPosX(menuOpenXPos, menuLerpDuration)
            .SetEase(Ease.InOutQuad); // Eases in and out smoothly
    }

    public void HideMenu()
    {
        menuRect.DOKill(); // Stop any existing tween
        menuRect.DOAnchorPosX(menuCloseXPos, menuLerpDuration)
            .SetEase(Ease.InOutQuad);
    }

    private void SequentiallyLoadButtons(List<Button> buttons)
    {
        float startY = 0 + (buttons.Count - 3) * buttonYDiff;

        for (int i = 0; i < buttons.Count; i++)
        {
            RectTransform btnRect = buttons[i].GetComponent<RectTransform>();
            float targetX = minButtonXPos - (buttonXDiff * i);
            float targetY = startY - (buttonYDiff * i);

            btnRect.DOKill(); // Stop any existing tween
            btnRect.DOAnchorPos(new Vector2(targetX, targetY), menuLerpDuration)
                .SetEase(Ease.InOutQuad)
                .SetDelay(i * delayBetweenButtons); // Delay each button animation
        }
    }

    private void ResetButtonPositions()
    {
        foreach (var button in titleButtonList.Concat(gameModeButtonList)
                                              .Concat(inGameButtonList)
                                              .Concat(settingsButtonList))
        {
            if (button != null)
            {
                RectTransform rectTransform = button.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.DOKill(); // Stop any ongoing animation
                    rectTransform.anchoredPosition = new Vector2(500, 0); // Default hidden button location
                }
            }
        }
    }


    public void ToggleMenu()
    {
        if (menuState != MenuType.None && gameState != GameState.TitleScreen)
        {
            // Hide menu
            HideMenu();
            menuState = MenuType.None;
        }
        else if (gameState == GameState.Playing || gameState == GameState.GameOver)
        {
            // Show in-game menu
            ShowMenu();
            ShowInGameMenu();
        }
    }
}