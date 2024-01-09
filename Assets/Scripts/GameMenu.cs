using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public GameObject menu;


    // Start is called before the first frame update
    void Start()
    {
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

    //private void ResetGameValues()
    //{
    //    playerOneMovement.SetIsServing(true);
    //    playerTwoMovement.SetIsServing(false);

    //    playerOneTransform.position = new Vector3(-playerOneMovement.startingXCoord, playerOneTransform.position.y, playerOneTransform.position.z);
    //    playerTwoTransform.position = new Vector3(playerTwoMovement.startingXCoord, playerTwoTransform.position.y, playerTwoTransform.position.z);

    //    scoreManager.ResetScores();
    //}

    public void ShowMenu(bool showMenu)
    {
        menu.SetActive(showMenu);
        //playerOneMovement.SetIsControlEnabled(false);
        //playerTwoMovement.SetIsControlEnabled(false);
    }


}
