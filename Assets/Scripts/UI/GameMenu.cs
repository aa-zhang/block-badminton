using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }

    public delegate void GameHandler(int trainingEnvId);
    public static GameHandler OnGameRestart;

    [SerializeField] private bool showOnStart = false;

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
            instance.ShowMenu(showOnStart);
        }
    }

    public void PlayOfflineGame()
    {
        SceneManager.LoadScene(1);
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
        OnGameRestart(trainingEnvId);
    }

    public void ShowMenu(bool showMenu)
    {
        gameObject.SetActive(showMenu);
        //playerOneMovement.SetIsControlEnabled(false);
        //playerTwoMovement.SetIsControlEnabled(false);
    }

    public void ToggleMenu()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}