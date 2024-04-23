using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServeController : NetworkBehaviour
{
    public bool isServing = false;
    private PlayerManager playerManager;
    private Transform playerTransform;

    private Transform birdieTransform;
    private BirdieMovement birdieMovement;
    private BirdieParticleController birdiePsController;

    // Start is called before the first frame update
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerTransform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (isServing && IsLocalPlayer)
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

    [Rpc(SendTo.Server)]
    public void HoldBirdieRpc()
    {
        // Move the birdie in front of the serving player
        Vector3 servingOffset = playerManager.playerNum == 1 ? Constants.servingOffsetOne : Constants.servingOffsetTwo;
        birdieTransform.position = playerTransform.position + servingOffset;
    }

    private void GameStateManager_OnBirdieInitialized(GameObject birdie)
    {
        // Attach spawned birdie to this script
        birdieTransform = birdie.transform;
        birdieMovement = birdie.GetComponent<BirdieMovement>();
        birdiePsController = birdie.GetComponent<BirdieParticleController>();
    }

    private void GameStateManager_OnBeginServe(int playerNum)
    {
        // Initiate serving sequence for given playerNum
        if (playerManager.playerNum == playerNum && IsLocalPlayer)
        {
            isServing = true;
            ResetServingPlayerPosition(playerNum);
            SetBirdieGravityRpc(false);
            SetIgnoreBirdieCollisionRpc(false);
        }   
    }


    [Rpc(SendTo.Server)]
    private void SetBirdieGravityRpc(bool enableGravity)
    {
        birdieMovement.SetEnableBirdieGravity(enableGravity);
    }

    [Rpc(SendTo.Server)]
    private void SetIgnoreBirdieCollisionRpc(bool ignoreCollision)
    {
        birdieMovement.SetIgnoreBirdieCollision(ignoreCollision);
    }

    private void ResetServingPlayerPosition(int playerNum)
        {
        // Move player behind the serving line
        float newXPos;
        if (playerNum == 1)
        {
            newXPos = Mathf.Min(playerTransform.position.x, -Constants.servingLineXPos);
        }
        else
        {
            newXPos = Mathf.Max(playerTransform.position.x, Constants.servingLineXPos);
        }

        playerTransform.position = new Vector3(newXPos, playerTransform.position.y, playerTransform.position.z);

    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        if (isServing)
        {
            // Start timer for black flash detection
            birdiePsController.ResetServeTimer();
        }
        isServing = false;
    }
}
