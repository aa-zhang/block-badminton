using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServeController : NetworkBehaviour
{
    public bool isServing = false;
    private PlayerManager playerManager;
    private Transform playerTransform;

    private GameObject birdie;
    private BirdieMovement birdieMovement;
    private BirdieParticleController birdiePsController;

    // Start is called before the first frame update
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerTransform = gameObject.transform;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Debug.Log(isServing);
        if (isServing && IsClient)
        {
            HoldBirdieRpc();
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

    [Rpc(SendTo.Server)]
    public void HoldBirdieRpc()
    {
        // Keep birdie infront of the player
        birdieMovement.SetServingPosition(playerTransform.position, playerManager.playerNum);
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        //if (isServing)
        //{
        //    // Start timer for black flash detection
        //    birdiePsController.ResetServeTimer();
        //}
        isServing = false;
        SetServeStatusRpc(false);
    }

    [Rpc(SendTo.Everyone)]
    public void SetServeStatusRpc(bool isServing)
    {
        // Keep birdie infront of the player
        this.isServing = isServing;
    }
}
