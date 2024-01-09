using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public GameObject playerOne;
    public GameObject playerTwo;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private bool playerControlsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        playerOneMovement = playerOne.GetComponent<PlayerMovement>();
        playerTwoMovement = playerTwo.GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
    }

    private void ReadInput()
    {
        if (playerControlsEnabled)
        {
            // Move left
            if (Input.GetKey(KeyCode.A))
            {
                playerOneMovement.MoveLeft();
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                playerTwoMovement.MoveLeft();
            }

            // Move right
            if (Input.GetKey(KeyCode.D))
            {
                playerOneMovement.MoveRight();
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                playerTwoMovement.MoveRight();
            }

            // Jump
            if (Input.GetKey(KeyCode.W))
            {
                playerOneMovement.Jump();
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                playerTwoMovement.Jump();
            }

            // Swing racket
            if (Input.GetKey(KeyCode.S))
            {
                playerOneMovement.SwingRacket();
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                playerTwoMovement.SwingRacket();
            }
        }
    }
}
