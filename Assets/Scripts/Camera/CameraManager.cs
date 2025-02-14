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
        GameMenu.OnGameStart += GameMenu_OnGameStart;
        GameMenu.OnReturnToTitleScreen += GameMenu_OnReturnToTitleScreen;
    }

    private void OnDisable()
    {
        GameMenu.OnGameStart -= GameMenu_OnGameStart;
        GameMenu.OnReturnToTitleScreen -= GameMenu_OnReturnToTitleScreen;
    }

    private void GameMenu_OnGameStart(PlayMode playMode)
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
