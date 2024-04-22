using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class GameStateManager : NetworkBehaviour
{
    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;
    public TextMeshProUGUI readyCountText;

    private int numPlayersJoined = 0;
    private NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private int scoringPlayerNum;

    private bool hasGameStarted = false;

    [SerializeField] private int winningScore = 11; // Default winning score
    [SerializeField] private int maxScore = 15; // Deuce score cap

    // Things needed for initializing the birdie
    [SerializeField] private GameObject birdiePrefab;

    public delegate void BirdieObjectHandler(GameObject birdie);
    public static BirdieObjectHandler OnBirdieInitialized;

    public delegate void ServeHandler(int playerNum);
    public static ServeHandler OnBeginServe;


    // Start is called before the first frame update
    void Start()
    {
        menu = canvas.GetComponent<GameMenu>();
        menu.ShowMenu(false);
        winnerText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void FixedUpdate()
    {
        if (!hasGameStarted)
        {
            ShowReadyCount();
        }
        else
        {
            CheckScore();
            DisplayScore();
        }
    }

    private void OnEnable()
    {
        NetworkManagerUI.OnPlayerSpawned += NetworkManagerUI_OnPlayerSpawned;
        BirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
    }

    private void OnDisable()
    {
        NetworkManagerUI.OnPlayerSpawned -= NetworkManagerUI_OnPlayerSpawned;
        BirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
    }

    private void NetworkManagerUI_OnPlayerSpawned()
    {
        numPlayersJoined++;
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (scoringPlayerNum == 1)
        {
            playerOneScore.Value++;
        }
        else
        {
            playerTwoScore.Value++;
        }
        this.scoringPlayerNum = scoringPlayerNum;

        // Start next serve after a 1 second delay
        Invoke("BeginNextServe", 1);
    }

    private void BeginNextServe()
    {
        // This helper method is used in order to allow for a delay using Invoke()
        OnBeginServe(scoringPlayerNum);
    }

    

    private void ShowReadyCount()
    {
        if (numPlayersJoined >= 2)
        {
            InitiateMatch();
            hasGameStarted = true;
        }
    }

    private void InitiateMatch()
    {
        Debug.Log("Game starting!");
        readyCountText.gameObject.SetActive(false);
        // Spawn birdie
        Debug.Log("Creating birdie");
        GameObject spawnedBirdieGameObject = Instantiate(birdiePrefab);
        NetworkObject birdieNetworkObject = spawnedBirdieGameObject.GetComponent<NetworkObject>();
        birdieNetworkObject.Spawn(true);


        InitialzeBirdieClientRpc(birdieNetworkObject.NetworkObjectId);
        
        Invoke("SelectRandomServer", 1);
    }

    [ClientRpc]
    private void InitialzeBirdieClientRpc(ulong birdieNetworkObjectId)
    {
        NetworkObject foundObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[birdieNetworkObjectId];
        OnBirdieInitialized(foundObj.gameObject);
    }


    private void SelectRandomServer()
    {
        // Select random number from {1, 2}
        //int playerNum = Random.Range(1, 3);
        int playerNum = 1;
        OnBeginServe(playerNum);
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore.Value, playerTwoScore.Value);
        int losingPlayerScore = Mathf.Min(playerOneScore.Value, playerTwoScore.Value);

        // Check if the match is in a Duece or Match Point state
        if (winningPlayerScore >= winningScore - 1)
        {
            if (winningPlayerScore == losingPlayerScore)
            {
                matchText.text = "Deuce";
            }
            else
            {
                matchText.text = "Match Point";
            } 
        }


        // Check if a player has won
        if ((winningPlayerScore >= winningScore && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == maxScore)
        {
            matchText.text = "Match Over!";

            if (playerOneScore.Value >= winningScore)
            {
                winnerText.text = "The strongest badminton player in history wins!";
                menu.ShowMenu(true);
                winnerText.gameObject.SetActive(true);
            }
            else if (playerTwoScore.Value >= winningScore)
            {
                winnerText.text = "The strongest badminton player of today wins!";
                menu.ShowMenu(true);
                winnerText.gameObject.SetActive(true);
            }
        }
        
    }    

    private void DisplayScore()
    {
        scoreText.text = playerOneScore.Value + " - " + playerTwoScore.Value;
    }


    public void ResetScores()
    {
        playerOneScore.Value = 0;
        playerTwoScore.Value = 0;
    }
}
