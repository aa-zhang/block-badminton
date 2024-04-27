using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class GameStateManager : NetworkBehaviour
{
    // UI elements
    private GameMenu menu;
    public GameObject canvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;
    public TextMeshProUGUI readyCountText;

    // Score variables
    private NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private int scoringPlayerNum;
    private bool hasGameStarted = false;


    // Network
    private List<ulong> clientIds = new List<ulong>();
    [SerializeField] private GameObject birdiePrefab;
    private NetworkObject birdieNetworkObject;

    // Events
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

    private void FixedUpdate()
    {
        if (!hasGameStarted)
        {
            WaitForAllPlayersToJoin();
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

    private void NetworkManagerUI_OnPlayerSpawned(ulong clientId)
    {
        clientIds.Add(clientId);
    }
    

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (!IsServer) return; // Only let server handle score

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
        BeginServeRpc(scoringPlayerNum);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void BeginServeRpc(int playerNum)
    {
        OnBeginServe(playerNum);
    }

    private void WaitForAllPlayersToJoin()
    {
        if (clientIds.Count >= 2)
        {
            InitiateMatch();
            SetGameStartedRpc(true);
        }
    }

    [Rpc(SendTo.Everyone)]
    private void SetGameStartedRpc(bool hasGameStarted)
    {
        this.hasGameStarted = hasGameStarted;
    }


    private void InitiateMatch()
    {
        Debug.Log("Game starting!");
        readyCountText.gameObject.SetActive(false);

        Debug.Log("Creating birdie");
        GameObject spawnedBirdieGameObject = Instantiate(birdiePrefab);
        birdieNetworkObject = spawnedBirdieGameObject.GetComponent<NetworkObject>();
        birdieNetworkObject.Spawn(true);

        // Initialize birdie for clients
        InitializeBirdieRpc(birdieNetworkObject.NetworkObjectId);
        
        Invoke("SelectRandomServer", 1);
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void InitializeBirdieRpc(ulong birdieNetworkObjectId)
    {
        // Find birdie network object
        NetworkObject foundObj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[birdieNetworkObjectId];
        // Call event to attach birdie to other scripts
        OnBirdieInitialized(foundObj.gameObject);
    }

    private void SelectRandomServer()
    {
        // Select random number from {1, 2}
        int playerNum = Random.Range(1, 3);
        BeginServeRpc(playerNum);
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore.Value, playerTwoScore.Value);
        int losingPlayerScore = Mathf.Min(playerOneScore.Value, playerTwoScore.Value);

        // Check if the match is in a Deuce or Match Point state
        if (winningPlayerScore >= Constants.winningScore - 1)
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
        if ((winningPlayerScore >= Constants.winningScore && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.maxScore)
        {
            matchText.text = "Match Over!";

            if (playerOneScore.Value >= Constants.winningScore)
            {
                winnerText.text = "The strongest badminton player in history wins!";
                menu.ShowMenu(true);
                winnerText.gameObject.SetActive(true);
            }
            else if (playerTwoScore.Value >= Constants.winningScore)
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
