using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject menu;

    public delegate void GameHandler();
    public static GameHandler OnGameRestart;

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
        Debug.Log("setting menu to " + showMenu);
        menu.SetActive(showMenu);
        //playerOneMovement.SetIsControlEnabled(false);
        //playerTwoMovement.SetIsControlEnabled(false);
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
    }


}
