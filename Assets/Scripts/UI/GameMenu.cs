using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using DG.Tweening;

public enum MenuType { TitleScreen, GameModeSelection, InGame, Settings, Credits, None }



public class GameMenu : MonoBehaviour
{
    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }


    public static Action OnReturnToTitleScreen;

    public MenuType menuState = MenuType.TitleScreen;
    [SerializeField] private float menuOpenXPos = -70f;
    [SerializeField] private float menuCloseXPos = 1150f;
    [SerializeField] private float menuLerpDuration = 0.25f;
    [SerializeField] private float delayAfterSplashScreen = 0.5f;
    [SerializeField] private int fpsCap = 90;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject version;
    [SerializeField] private GameObject playHeader;
    [SerializeField] private GameObject settingsHeader;
    [SerializeField] private GameObject creditsHeader;
    [SerializeField] private GameObject creditsContent;
    [SerializeField] private GameObject optionsHeader;

    private ButtonAnimator buttonAnimator;

    private GameState gameState = GameState.NotPlaying;

    private RectTransform menuRect;

    private void Awake()
    {
        Application.targetFrameRate = fpsCap;

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
        menuRect = GetComponent<RectTransform>();
        buttonAnimator = GetComponentInChildren<ButtonAnimator>();

        StartCoroutine(WaitForSplashScreenToFinish());
    }

    private void OnEnable()
    {
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
    }

    private IEnumerator WaitForSplashScreenToFinish()
    {
        while (!SplashScreen.isFinished)
        {
            yield return null;
        }
        // Wait for an additional delay after the splash screen finishes
        yield return new WaitForSeconds(delayAfterSplashScreen);
        SetMenuOptions(MenuType.TitleScreen);
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState state)
    {
        gameState = state;
    }


    public void ReturnToStartScreen()
    {
        Time.timeScale = 1f; // Resume time (for camera to be able to move)
        OnReturnToTitleScreen?.Invoke();
        gameState = GameState.NotPlaying;
        ShowMenu(MenuType.TitleScreen);
    }

    public void QuitGame()
    {
        Application.Quit();
    }


    public void SetMenuOptions(MenuType menuType)
    {
        // Toggle UI elements based on the selected menu
        title.SetActive(menuType == MenuType.TitleScreen);
        version.SetActive(menuType == MenuType.TitleScreen);
        playHeader.SetActive(menuType == MenuType.GameModeSelection);
        settingsHeader.SetActive(menuType == MenuType.Settings);
        creditsHeader.SetActive(menuType == MenuType.Credits);
        creditsContent.SetActive(menuType == MenuType.Credits);
        optionsHeader.SetActive(menuType == MenuType.InGame);

        buttonAnimator.LoadButtons(menuType);

        menuState = menuType; // Update menu state
    }


    public void ShowPreviousMenu()
    {
        // This method is called when the Back button is pressed
        MenuType previousMenu;

        if (gameState == GameState.Rallying || gameState == GameState.Serving || gameState == GameState.GameOver)
        {
            previousMenu = MenuType.InGame;
        }
        else if (gameState == GameState.MatchOver)
        {
            // TODO: show match menue (same as game menu, but remove the resume button)
            previousMenu = MenuType.InGame;
        }
        else
        {
            previousMenu = MenuType.TitleScreen;
        }

        ShowMenu(previousMenu);
    }

    public void ShowMenu(MenuType menuType)
    {
        menuRect.DOKill(); // Stop any existing tween
        menuRect.DOAnchorPosX(menuOpenXPos, menuLerpDuration)
            .SetEase(Ease.InOutQuad) // Eases in and out smoothly
            .SetUpdate(true); // Runs even when time is stopped

        SetMenuOptions(menuType);
    }

    // Use these for button actions (since Unity won't let me call a method that has an Enum parameter)
    public void ShowGameModesMenu() => ShowMenu(MenuType.GameModeSelection);
    public void ShowSettingsMenu() => ShowMenu(MenuType.Settings);
    public void ShowCreditsMenu() => ShowMenu(MenuType.Credits);



    public void HideMenu()
    {
        menuRect.DOKill(); // Stop any existing tween
        menuRect.DOAnchorPosX(menuCloseXPos, menuLerpDuration)
            .SetEase(Ease.InOutQuad);

        menuState = MenuType.None;
    }

    public void ToggleMenu()
    {
        if (menuState != MenuType.None && gameState != GameState.NotPlaying)
        {
            HideMenu();
            Time.timeScale = 1f; // Resume the game
        }
        else if (gameState == GameState.Rallying || gameState == GameState.Serving || gameState == GameState.GameOver)
        {
            ShowMenu(MenuType.InGame);
            Time.timeScale = 0; // Freeze the game
        }
        else if (gameState == GameState.MatchOver)
        {
            // TODO: show match menue (same as game menu, but remove the resume button)
            ShowMenu(MenuType.InGame);
        }
    }
}