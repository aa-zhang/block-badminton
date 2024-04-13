using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;


public class InputManager : MonoBehaviour
{
    private GameObject playerOne;
    private GameObject playerTwo;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private PhotonView photonView;


    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI pingText;

    private bool playerControlsEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        menu = canvas.GetComponent<GameMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        ReadInput();
        DisplayPing();
    }

    public void InitializeOnlinePlayer(GameObject player)
    {
        photonView = player.GetComponent<PhotonView>();

        playerOneMovement = player.GetComponent<PlayerMovement>();
        playerTwoMovement = player.GetComponent<PlayerMovement>();
    }


    private void ReadInput()
    {
        // Player Movement Controls
        if (playerControlsEnabled && photonView.IsMine)
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

        // UI Controls
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.ToggleMenu();
        }
    }

    public void SetPlayerControlsEnabled(bool enabled)
    {
        playerControlsEnabled = enabled; // TODO: replace this with an open menu event
    }

    private void DisplayPing()
    {
        pingText.text = "Ping: " + PhotonNetwork.GetPing();
    }
}
