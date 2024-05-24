using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OfflineInputManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private bool playerControlsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = GetComponent<PlayerManager>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }


    private void ReadInput()
    {
        // Player Movement Controls
        if (playerControlsEnabled)
        {
            // Move left
            if (Input.GetKeyDown(playerManager.leftKey))
            {
                //playerMovement.MoveLeft();
                playerMovement.DashLeft();
            }

            // Move right
            if (Input.GetKeyDown(playerManager.rightKey))
            {
                //playerMovement.MoveRight();
                playerMovement.DashRight();
            }

            // Jump
            if (Input.GetKey(playerManager.jumpKey))
            {
                playerMovement.Jump();
            }

            // Swing racket
            if (Input.GetKey(playerManager.lightSwingKey))
            {
                playerMovement.SwingRacket(SwingType.Light);
            }

            if (Input.GetKey(playerManager.heavySwingKey))
            {
                playerMovement.SwingRacket(SwingType.Heavy);
            }

            // Change Serve Angle
            if (Input.GetKeyDown(playerManager.jumpKey))
            {
                playerMovement.SetServeAngle(ServeAngle.High);
            }
            if (Input.GetKeyDown(playerManager.downKey))
            {
                playerMovement.SetServeAngle(ServeAngle.Low);
            }
        }

        // UI Controls
        if (Input.GetKeyDown(KeyCode.Escape) && playerManager.playerNum == 1)
        {
            //playerControlsEnabled = enabled;
            GameMenu.Instance.ToggleMenu();
        }
    }

}
