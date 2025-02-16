using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;


public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera startCamera;
    [SerializeField] private CinemachineVirtualCamera gameCamera;

    private void OnEnable()
    {
        PlayerLoader.OnPlayersLoaded += PlayerLoader_OnPlayersLoaded;
        GameMenu.OnReturnToTitleScreen += GameMenu_OnReturnToTitleScreen;
    }

    private void OnDisable()
    {
        PlayerLoader.OnPlayersLoaded -= PlayerLoader_OnPlayersLoaded;
        GameMenu.OnReturnToTitleScreen -= GameMenu_OnReturnToTitleScreen;
    }

    private void PlayerLoader_OnPlayersLoaded(PlayMode playMode)
    {
        SwitchToGameCamera();
    }

    private void GameMenu_OnReturnToTitleScreen()
    {
        SwitchToStartCamera();
    }

    public void SwitchToGameCamera()
    {
        gameCamera.Priority = 10;
        startCamera.Priority = 5;
    }

    // Switch to Camera 2
    public void SwitchToStartCamera()
    {
        gameCamera.Priority = 5;
        startCamera.Priority = 10;
    }

}
