using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;



public class GameMenu : MonoBehaviour
{
    public enum MenuType { TitleScreen, GameModeSelection, InGame, Settings, None }

    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }

    public static Action OnGameStart;
    public static Action<int> OnGameRestart;

    [SerializeField] private MenuType menuState = MenuType.TitleScreen;
    [SerializeField] private float menuOpenXPos = -70f;
    [SerializeField] private float menuCloseXPos = 1150f;
    [SerializeField] private float menuLerpDuration = 0.25f;

    [SerializeField] private GameObject title;
    [SerializeField] private GameObject version;

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
        StartCoroutine(SequentiallyLoadButtons(titleButtonList));
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
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGameValues(int trainingEnvId)
    {
        OnGameRestart?.Invoke(trainingEnvId);
    }


    private IEnumerator ShowMenu()
    {
        float startX = menuRect.anchoredPosition.x;
        float t = 0;

        while (t < 1.0f) // Use < instead of <= to prevent overshooting
        {
            t += Time.deltaTime / menuLerpDuration;
            float easedT = Mathf.SmoothStep(0.0f, 1.0f, t);
            float newX = Mathf.Lerp(startX, menuOpenXPos, easedT);

            menuRect.anchoredPosition = new Vector2(newX, menuRect.anchoredPosition.y);
            yield return null;
        }

        menuRect.anchoredPosition = new Vector2(menuOpenXPos, menuRect.anchoredPosition.y);
    }

    private IEnumerator HideMenu()
    {
        float startX = menuRect.anchoredPosition.x;
        float t = 0;

        while (t < 1.0f)
        {
            t += Time.deltaTime / menuLerpDuration;
            float easedT = Mathf.SmoothStep(0.0f, 1.0f, t);
            float newX = Mathf.Lerp(startX, menuCloseXPos, easedT);

            menuRect.anchoredPosition = new Vector2(newX, menuRect.anchoredPosition.y);
            yield return null;
        }

        menuRect.anchoredPosition = new Vector2(menuCloseXPos, menuRect.anchoredPosition.y);
    }

    private IEnumerator SequentiallyLoadButtons(List<Button> buttons)
    {
        int buttonCounter = 0;
        float startY = 0 + (buttons.Count - 3) * buttonYDiff;
        foreach (Button btn in buttons)
        {
            RectTransform btnRect = btn.GetComponent<RectTransform>();
            float targetX = minButtonXPos - (buttonXDiff * buttonCounter);
            float targetY = startY - (buttonYDiff * buttonCounter);
            StartCoroutine(LerpButtonPosition(btnRect, targetX, targetY));
            buttonCounter++;
            yield return new WaitForSeconds(delayBetweenButtons); // Wait before moving next button
        }
    }

    private IEnumerator LerpButtonPosition(RectTransform rectTransform, float targetX, float targetY)
    {
        float elapsed = 0f;
        Vector2 startPos = new Vector2(rectTransform.anchoredPosition.x, targetY);
        Vector2 targetPos = new Vector2(targetX, targetY);

        while (elapsed < menuLerpDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / menuLerpDuration;
            rectTransform.anchoredPosition = Vector2.Lerp(startPos, targetPos, Mathf.SmoothStep(0, 1, t));
            yield return null;
        }

        rectTransform.anchoredPosition = targetPos; // Ensure final position is exact
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
            StartCoroutine(HideMenu());
            menuState = MenuType.None;
            ResetButtonPositions();
        }
        else if (gameState == GameState.Playing || gameState == GameState.GameOver)
        {
            // Show in-game menu
            title.SetActive(false);
            version.SetActive(false);

            StartCoroutine(ShowMenu());
            StartCoroutine(SequentiallyLoadButtons(inGameButtonList));
            menuState = MenuType.InGame;
        }
    }
}