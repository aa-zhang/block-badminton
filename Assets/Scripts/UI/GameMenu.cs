using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }

    public static Action OnGameStart;
    public static Action<int> OnGameRestart;

    [SerializeField] private bool isMenuShowing = true;
    [SerializeField] private float menuOpenXPos = -70f;
    [SerializeField] private float menuCloseXPos = 1150f;
    [SerializeField] private float menuLerpDuration = 0.25f;


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
            //instance.ShowMenu(showOnStart);
        }
    }

    public void PlayOfflineGame()
    {
        OnGameStart?.Invoke();
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


    IEnumerator ShowMenu()
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

    IEnumerator HideMenu()
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


    public void ToggleMenu()
    {
        if (isMenuShowing)
        {
            StartCoroutine(HideMenu());
        }
        else
        {
            StartCoroutine(ShowMenu());
        }
        isMenuShowing = !isMenuShowing;
    }
}