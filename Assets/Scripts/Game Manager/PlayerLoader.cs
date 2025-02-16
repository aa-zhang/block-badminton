using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerOne;
    [SerializeField] private GameObject playerTwo;
    [SerializeField] private GameObject aiPlayerOne;
    [SerializeField] private GameObject aiPlayerTwo;

    public static Action<PlayMode> OnPlayersLoaded;

    
    private void OnEnable()
    {
        GameModeManager.OnGameStartRequested += GameModeManager_OnGameStartRequested;
    }

    private void OnDisable()
    {
        GameModeManager.OnGameStartRequested -= GameModeManager_OnGameStartRequested;
    }

    private void GameModeManager_OnGameStartRequested(PlayMode playMode)
    {
        playerTwo.SetActive(playMode == PlayMode.Human);
        aiPlayerTwo.SetActive(playMode == PlayMode.AI);
        OnPlayersLoaded?.Invoke(playMode);
    }
}
