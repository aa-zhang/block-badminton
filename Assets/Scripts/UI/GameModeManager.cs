using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayMode { Human, AI }

public class GameModeManager : MonoBehaviour
{
    public static Action<PlayMode> OnGameStart;

    public void PlayOfflineGame()
    {
        OnGameStart?.Invoke(PlayMode.Human);
        GameMenu.Instance.ToggleMenu();
    }

    public void PlayAIGame()
    {
        OnGameStart?.Invoke(PlayMode.AI);
        GameMenu.Instance.ToggleMenu();
    }

    public void PlayOnlineGame()
    {
    }

}
