using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject playerOnePrefab;
    public GameObject playerTwoPrefab;
    public GameObject birdiePrefab;
    private Vector3 playerOneSpawnPosition = new Vector3(-6, 2, 0);
    private Vector3 playerTwoSpawnPosition = new Vector3(6, 2, 0);
    private Vector3 birdieSpawnPosition = new Vector3(0, 10, 0);

    public InputManager inputManager;


    // Start is called before the first frame update
    void Start()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;

        if (playerCount == 1)
        {
            GameObject birdie = PhotonNetwork.Instantiate(birdiePrefab.name, birdieSpawnPosition, Quaternion.identity);

            GameObject onlinePlayerOne = PhotonNetwork.Instantiate(playerOnePrefab.name, playerOneSpawnPosition, Quaternion.identity);
            inputManager.initializeOnlinePlayer(onlinePlayerOne);

            onlinePlayerOne.GetComponent<SwingRacket>().InitializeBirdie(birdie);
        }
        else if (playerCount == 2)
        {
            GameObject onlinePlayerTwo = PhotonNetwork.Instantiate(playerTwoPrefab.name, playerTwoSpawnPosition, Quaternion.Euler(0, 180, 0));
            inputManager.initializeOnlinePlayer(onlinePlayerTwo);

        }
        else
        {
            Debug.Log("Lobby is full. Can't spawn player.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
