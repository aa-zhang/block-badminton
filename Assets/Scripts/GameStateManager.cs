using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameStateManager : MonoBehaviour
{
    public GameObject canvas;
    private GameMenu menu;

    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI winnerText;
    public TextMeshProUGUI matchText;
    public TextMeshProUGUI readyCountText;

    public int playerOneScore = 0;
    public int playerTwoScore = 0;
    private int scoringPlayerNum;

    private bool hasGameStarted = false;

    [SerializeField] private int winningScore = 11; // Default winning score
    [SerializeField] private int maxScore = 15; // Deuce score cap

    // Things needed for initializing the birdie
    public GameObject birdiePrefab;
    private Vector3 birdieSpawnPosition = new Vector3(0, 10, 0);

    public delegate void BirdieObjectHandler(GameObject birdie);
    public static BirdieObjectHandler OnBirdieInitialized;

    public delegate void ServeHandler(int playerNum);
    public static ServeHandler OnBeginServe;

    private PhotonView photonView;


    // Start is called before the first frame update
    void Start()
    {
        menu = canvas.GetComponent<GameMenu>();
        menu.ShowMenu(false);
        winnerText.gameObject.SetActive(false);
        photonView = gameObject.GetPhotonView();
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
        BirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        BirdieMovement.OnOwnershipTransferInitiated += BirdieMovement_OnOwnershipTransferInitiated;
    }

    private void OnDisable()
    {
        BirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        BirdieMovement.OnOwnershipTransferInitiated -= BirdieMovement_OnOwnershipTransferInitiated;
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (scoringPlayerNum == 1)
        {
            playerOneScore++;
        }
        else
        {
            playerTwoScore++;
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

    private void BirdieMovement_OnOwnershipTransferInitiated(PhotonView birdieView)
    {
        // Transfers ownership of the birdie's PhotonView to the player who initiated the collision.

        // TODO: maybe change it to assign ownership of the birdie's PhotonView to the player about to hit it,
        // rather than the player who just hit it, can enhance synchronization for smoother gameplay.

        Player[] players = PhotonNetwork.PlayerList;
        foreach (Player player in players)
        {
            if (player.IsLocal)
            {
                birdieView.TransferOwnership(player);
                break;
            }
        }
    }

    private void ShowReadyCount()
    {
        int playerCount = PhotonNetwork.CurrentRoom.PlayerCount;
        readyCountText.text = $"{playerCount}/2 Players joined";

        if (playerCount >= 2)
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
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("Creating birdie");
            GameObject birdie = PhotonNetwork.InstantiateRoomObject(birdiePrefab.name, birdieSpawnPosition, Quaternion.identity);
            // Call an RPC to notify all players about the initialized birdie
            photonView.RPC("NotifyBirdieInitialization", RpcTarget.All, birdie.GetComponent<PhotonView>().ViewID);

            OnBirdieInitialized(birdie);
            OnBeginServe(1);
        }
    }

    [PunRPC]
    private void NotifyBirdieInitialization(int birdieViewID)
    {
        // Find the GameObject with the specified PhotonViewID
        GameObject birdie = PhotonView.Find(birdieViewID).gameObject;

        OnBirdieInitialized(birdie);
    }

    private void CheckScore()
    {
        int winningPlayerScore = Mathf.Max(playerOneScore, playerTwoScore);
        int losingPlayerScore = Mathf.Min(playerOneScore, playerTwoScore);

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

            if (playerOneScore >= winningScore)
            {
                winnerText.text = "The strongest badminton player in history wins!";
                menu.ShowMenu(true);
                winnerText.gameObject.SetActive(true);
            }
            else if (playerTwoScore >= winningScore)
            {
                winnerText.text = "The strongest badminton player of today wins!";
                menu.ShowMenu(true);
                winnerText.gameObject.SetActive(true);
            }
        }
        
    }    

    private void DisplayScore()
    {
        scoreText.text = playerOneScore + " - " + playerTwoScore;
    }


    public void ResetScores()
    {
        playerOneScore = 0;
        playerTwoScore = 0;
    }
}
