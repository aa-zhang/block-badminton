using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class InputManager : NetworkBehaviour
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
        if (playerControlsEnabled && IsOwner)
        {
            // Move left
            if (Input.GetKey(playerManager.leftKey))
            {
                playerMovement.MoveLeft();
            }

            // Move right
            if (Input.GetKey(playerManager.rightKey))
            {
                playerMovement.MoveRight();
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
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //playerControlsEnabled = enabled;
            GameMenu.Instance.ToggleMenu();
        }
    }

}
