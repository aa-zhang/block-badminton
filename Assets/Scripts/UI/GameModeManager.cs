using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayMode { Human, AI }

public class GameModeManager : MonoBehaviour
{
    public static Action<PlayMode> OnGameStartRequested;
    private PlayMode playMode;

    public void PlayOfflineGame()
    {
        OnGameStartRequested?.Invoke(PlayMode.Human);
        playMode = PlayMode.Human;
        GameMenu.Instance.ToggleMenu();
    }

    public void PlayAIGame()
    {
        OnGameStartRequested?.Invoke(PlayMode.AI);
        playMode = PlayMode.AI;
        GameMenu.Instance.ToggleMenu();
    }

    public void PlayOnlineGame()
    {
    }

    public void RestartGame(bool toggleMenu)
    {
        OnGameStartRequested?.Invoke(playMode);
        if (toggleMenu)
        {
            GameMenu.Instance.ToggleMenu();
        }
    }
}
