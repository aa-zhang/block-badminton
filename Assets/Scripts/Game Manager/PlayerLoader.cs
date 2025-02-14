using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerOne;
    [SerializeField] private GameObject playerTwo;
    [SerializeField] private GameObject aiPlayerOne;
    [SerializeField] private GameObject aiPlayerTwo;
    // Start is called before the first frame update
    
    private void OnEnable()
    {
        GameMenu.OnGameStart += GameMenu_OnGameStart;
    }

    private void OnDisable()
    {
        GameMenu.OnGameStart -= GameMenu_OnGameStart;
    }

    private void GameMenu_OnGameStart(PlayMode playMode)
    {
        playerTwo.SetActive(playMode == PlayMode.Human);
        aiPlayerTwo.SetActive(playMode == PlayMode.AI);
    }
}
