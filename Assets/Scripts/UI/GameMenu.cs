using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using DG.Tweening;

public enum MenuType { TitleScreen, GameModeSelection, InGame, Settings, None }



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

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject version;
    [SerializeField] private GameObject playText;
    [SerializeField] private GameObject settingsText;
    [SerializeField] private GameObject optionsText;

    private ButtonAnimator buttonAnimator;

    private GameState gameState = GameState.NotPlaying;
    private int currentQualityLevel;
    


    private RectTransform menuRect;

    private void Awake()
    {
        Application.targetFrameRate = 60;

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
        currentQualityLevel = QualitySettings.GetQualityLevel();
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
        playText.SetActive(menuType == MenuType.GameModeSelection);
        settingsText.SetActive(menuType == MenuType.Settings);
        optionsText.SetActive(menuType == MenuType.InGame);

        buttonAnimator.LoadButtons(menuType);

        menuState = menuType; // Update menu state
    }


    public void ShowPreviousMenu()
    {
        // This method is called when the Back button is pressed
        MenuType previousMenu;

        if (gameState == GameState.Playing || gameState == GameState.Serving || gameState == GameState.GameOver)
        {
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
            .SetEase(Ease.InOutQuad); // Eases in and out smoothly

        SetMenuOptions(menuType);
    }

    // Use these for button actions (since Unity won't let me call a method that has an Enum parameter)
    public void ShowGameModesMenu() => ShowMenu(MenuType.GameModeSelection);
    public void ShowSettingsMenu() => ShowMenu(MenuType.Settings);


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
        }
        else if (gameState == GameState.Playing || gameState == GameState.Serving || gameState == GameState.GameOver)
        {
            ShowMenu(MenuType.InGame);
        }
    }
}