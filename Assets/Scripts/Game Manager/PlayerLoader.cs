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
        GameModeManager.OnGameStart += GameModeManager_OnGameStart;
    }

    private void OnDisable()
    {
        GameModeManager.OnGameStart -= GameModeManager_OnGameStart;
    }

    private void GameModeManager_OnGameStart(PlayMode playMode)
    {
        playerTwo.SetActive(playMode == PlayMode.Human);
        aiPlayerTwo.SetActive(playMode == PlayMode.AI);
    }
}
