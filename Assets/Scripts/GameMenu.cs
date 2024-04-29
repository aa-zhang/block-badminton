using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private static GameMenu instance;
    public static GameMenu Instance { get { return instance; } }

    public delegate void GameHandler();
    public static GameHandler OnGameRestart;

    [SerializeField] private bool showOnStart = false;


    private void Awake()
    {
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

    public void StartGame()
    {
        //ResetGameValues();
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGameValues()
    {
        // reset player location:?
        // initiate serve
        // reset score
        OnGameRestart();
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