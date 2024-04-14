using System.Collections;
using System.Collections.Generic;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

public class ServeController : MonoBehaviour
{
    public bool isServing = false;
    private PlayerManager playerManager;
    private Transform playerTransform;

    private GameObject birdie;
    private BirdieMovement birdieMovement;
    private BirdieParticleController birdiePsController;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerTransform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isServing)
        {
            HoldBirdie();
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnBirdieInitialized += GameStateManager_OnBirdieInitialized;
        GameStateManager.OnBeginServe += GameStateManager_OnBeginServe;
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
    }

    private void OnDisable()
    {
        GameStateManager.OnBirdieInitialized -= GameStateManager_OnBirdieInitialized;
        GameStateManager.OnBeginServe -= GameStateManager_OnBeginServe;
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
    }

    private void GameStateManager_OnBirdieInitialized(GameObject birdie)
    {
        this.birdie = birdie;
        birdieMovement = birdie.GetComponent<BirdieMovement>();
        birdiePsController = birdie.GetComponent<BirdieParticleController>();
    }

    private void GameStateManager_OnBeginServe(int playerNum)
    {
        // Initiate serving sequence for given playerNum
        if (playerManager.playerNum == playerNum)
        {
            isServing = true;
            ResetServingPlayerPosition();
        }   
    }

    private void ResetServingPlayerPosition()
    {
        // Move player behind the serving line
        float newXPos;
        if (playerManager.playerNum == 1)
        {
            newXPos = Mathf.Min(playerTransform.position.x, -Constants.servingLineXPos);
        }
        else
        {
            newXPos = Mathf.Max(playerTransform.position.x, Constants.servingLineXPos);
        }

        playerTransform.position = new Vector3(newXPos, playerTransform.position.y, playerTransform.position.z);

    }

    private void HoldBirdie()
    {
        // Keep birdie infront of the player
        birdieMovement.SetServingPosition(playerTransform.position, playerManager.playerNum);
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector)
    {
        if (isServing)
        {
            // Start timer for black flash detection
            birdiePsController.ResetServeTimer();
        }
        isServing = false;
    }
}
