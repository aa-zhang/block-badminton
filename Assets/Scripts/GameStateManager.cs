using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;


public class GameStateManager : NetworkBehaviour
{
    // UI elements
    public GameObject canvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;

    // Score variables
    private NetworkVariable<int> playerOneScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> playerTwoScore = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<int> servingPlayerNum = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private bool allPlayersJoined = false;
    private bool gameInProgress = false;


    // Network
    private List<ulong> clientIds = new List<ulong>();
    [SerializeField] private GameObject birdiePrefab;
    private NetworkObject birdieNetworkObject;

    // Events
    public delegate void BirdieObjectHandler(GameObject birdie);
    public static BirdieObjectHandler OnBirdieInitialized;

    public delegate void StartMatchHandler();
    public static StartMatchHandler OnStartMatch;

    public delegate void ServeHandler(int playerNum);
    public static ServeHandler OnBeginServe;


    // Start is called before the first frame update
    void Start()
    {
        ShowMenuRpc(false);
    }

    private void FixedUpdate()
    {
        if (!allPlayersJoined)
        {
            WaitForAllPlayersToJoin();
        }
        else
        {
            DisplayScore();
        }
    }

    private void OnEnable()
    {
        PlayerSpawner.OnPlayerSpawned += PlayerSpawner_OnPlayerSpawned;
        BirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
    }

    private void OnDisable()
    {
        PlayerSpawner.OnPlayerSpawned -= PlayerSpawner_OnPlayerSpawned;
        BirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
    }

    private void PlayerSpawner_OnPlayerSpawned(ulong clientId)
    {
        if (IsServer)
        {
            clientIds.Add(clientId);
        }
    }

    private void WaitForAllPlayersToJoin()
    {
        if (clientIds.Count >= 2)
        {
            InitiateGameRpc();
            SpawnNetworkBirdie();
        }
    }

    [Rpc(SendTo.Everyone)]
    private void InitiateGameRpc()
    {
        allPlayersJoined = true;
        gameInProgress = true;
        OnStartMatch();
    }


    private void SpawnNetworkBirdie()
    {
        Debug.Log("Game starting!");
        Debug.Log("Creating birdie");
        GameObject spawnedBirdieGameObject = Instantiate(birdiePrefab);
        birdieNetworkObject = spawnedBirdieGameObject.GetComponent<NetworkObject>();
        birdieNetworkObject.Spawn(true);

        // Initialize birdie for clients
        InitializeBirdieRpc(birdieNetworkObject.NetworkObjectId);
        
        Invoke("SelectRandomServer", 1);
    }

    [Rpc(SendTo.Everyone)]
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
        servingPlayerNum.Value = Random.Range(1, 3);
        BeginServeRpc();
    }

    [Rpc(SendTo.Everyone)]
    private void BeginServeRpc()
    {
        Debug.Log("serving player num: " + servingPlayerNum.Value);
        OnBeginServe(servingPlayerNum.Value);
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

        servingPlayerNum.Value = scoringPlayerNum;

        CheckScore(); // sets gameInProgress to false if game has ended

        if (gameInProgress)
        {
            // Start next serve after a 1 second delay
            Invoke("BeginServeRpc", 1);
        }
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
                SetMatchTextRpc("Deuce");
            }
            else
            {
                SetMatchTextRpc("Match Point");
            }
        }

        // Check if a player has won
        if ((winningPlayerScore >= Constants.winningScore && winningPlayerScore - losingPlayerScore >= 2) || winningPlayerScore == Constants.maxScore)
        {
            SetMatchTextRpc("Match Over!");
            gameInProgress = false;

            if (playerOneScore.Value >= Constants.winningScore)
            {
                SetWinnerTextRpc("The strongest badminton player in history wins!");
                ShowMenuRpc(true);
            }
            else if (playerTwoScore.Value >= Constants.winningScore)
            {
                SetWinnerTextRpc("The strongest badminton player of today wins!");
                ShowMenuRpc(true);
            }
        }
    }

    [Rpc(SendTo.Everyone)]
    private void ShowMenuRpc(bool show)
    {
        GameMenu.Instance.ShowMenu(show);

    }

    [Rpc(SendTo.Everyone)]
    private void SetMatchTextRpc(string text)
    {
        matchText.text = text;

    }

    [Rpc(SendTo.Everyone)]
    private void SetWinnerTextRpc(string text)
    {
        winnerText.text = text;
    }

    private void DisplayScore()
    {
        scoreText.text = playerOneScore.Value + " - " + playerTwoScore.Value;
    }

    private void GameMenu_OnGameRestart()
    {
        RestartGameRpc();
    }

    [Rpc(SendTo.Server)]
    private void RestartGameRpc()
    {
        // Reset text and score values
        SetMatchTextRpc("");
        SetWinnerTextRpc("");
        ShowMenuRpc(false);
        playerOneScore.Value = 0;
        playerTwoScore.Value = 0;
        InitiateGameRpc();
        Invoke("SelectRandomServer", 1);
    }
}
