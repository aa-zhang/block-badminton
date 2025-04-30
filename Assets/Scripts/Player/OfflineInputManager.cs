using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class OfflineInputManager : MonoBehaviour
{
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private bool playerControlsEnabled = true;

    private float leftLastPressTime;
    private float rightLastPressTime;
    private float DOUBLE_CLICK_TIME = 0.2f;



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
            if (Input.GetKey(playerManager.leftKey))
            {
                playerMovement.MoveLeft();
            }

            // Move right
            if (Input.GetKey(playerManager.rightKey))
            {
                playerMovement.MoveRight();
            }

            // Detect double press for dash left
            if (Input.GetKeyDown(playerManager.leftKey))
            {
                float timeSinceLastPress = Time.time - leftLastPressTime;
                if (timeSinceLastPress < DOUBLE_CLICK_TIME && leftLastPressTime > rightLastPressTime)
                {
                    playerMovement.DashLeft();
                }
                leftLastPressTime = Time.time;
            }

            // Detect double press for dash right
            if (Input.GetKeyDown(playerManager.rightKey))
            {
                float timeSinceLastPress = Time.time - rightLastPressTime;
                if (timeSinceLastPress < DOUBLE_CLICK_TIME && rightLastPressTime > leftLastPressTime)
                {
                    playerMovement.DashRight();
                }
                rightLastPressTime = Time.time;
            }

            // Jump
            if (Input.GetKey(playerManager.jumpKey))
            {
                playerMovement.Jump();
            }

            // Fast fall
            if (Input.GetKey(playerManager.downKey))
            {
                playerMovement.FastFall();
            }
            if (Input.GetKeyUp(playerManager.downKey))
            {
                playerMovement.CancelFastFall();
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
                playerMovement.ChangeServeAngle();
            }

        }
        
    }

}
