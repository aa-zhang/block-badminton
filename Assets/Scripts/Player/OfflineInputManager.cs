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
            if (Input.GetKey(playerManager.swingKey))
            {
                playerMovement.SwingRacket();
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
