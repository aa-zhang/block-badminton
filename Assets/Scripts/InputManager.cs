using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;


public class InputManager : NetworkBehaviour
{
    private PlayerMovement playerMovement;
    private bool playerControlsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
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
            if (Input.GetKey(KeyCode.A))
            {
                playerMovement.MoveLeft();
            }

            // Move right
            if (Input.GetKey(KeyCode.D))
            {
                playerMovement.MoveRight();
            }

            // Jump
            if (Input.GetKey(KeyCode.W))
            {
                playerMovement.Jump();
            }

            // Swing racket
            if (Input.GetKey(KeyCode.S))
            {
                playerMovement.SwingRacket();
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
