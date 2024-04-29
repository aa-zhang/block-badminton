using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private static GameMenu _instance;
    public static GameMenu Instance { get { return _instance; } }

    public delegate void GameHandler();
    public static GameHandler OnGameRestart;

    [SerializeField] private bool showOnStart = false;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            _instance.ShowMenu(showOnStart);
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