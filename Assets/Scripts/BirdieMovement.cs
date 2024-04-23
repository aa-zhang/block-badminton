using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BirdieMovement : NetworkBehaviour
{
    private Rigidbody birdieRb;
    private Transform birdieTransform;
    private bool enableGravity = false;

    private Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    private Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);

    public delegate void IncreaseScoreHandler(int scoringPlayerNum);
    public static IncreaseScoreHandler OnPointScored;


    // Start is called before the first frame update
    void Awake()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;

        SetIgnoreBirdieCollision(false);
    }

    private void FixedUpdate()
    {
        if (enableGravity)
        {
            ApplyGravity();
        }
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
        GameStateManager.OnBeginServe += GameStateManager_OnBeginServe;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        GameStateManager.OnBeginServe -= GameStateManager_OnBeginServe;
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        HitBirdieRpc(forceVector, playerNum);
    }

    [Rpc(SendTo.Server)]
    public void HitBirdieRpc(Vector3 forceVector, int playerNum)
    {
        if (!enableGravity)
        {
            enableGravity = true;
        }

        // Apply a force onto the birdie
        Debug.Log("adding force to birdie");
        birdieRb.velocity = Vector3.zero;
        birdieRb.AddForce(forceVector, ForceMode.Impulse);
    }

    private void GameStateManager_OnBeginServe(int playerNum)
    {
        BeginServeToServeRpc();
    }

    [Rpc(SendTo.Server)]
    private void BeginServeToServeRpc()
    {
        enableGravity = false;
        SetIgnoreBirdieCollision(false);
    }

    public void SetServingPosition(Vector3 playerPosition, int playerNum)
    {
        Vector3 servingOffset = playerNum == 1 ? servingOffsetOne : servingOffsetTwo;
        birdieTransform.position = playerPosition + servingOffset;
    }

    private void ApplyGravity()
    {
        if (IsServer)
        {
            birdieRb.AddForce(new Vector3(0, -4, 0));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor") && IsServer)
        {
            // Prevent players from hitting the birdie after it lands
            SetIgnoreBirdieCollision(true);

            // Determine if player 1 or 2 should receive the point
            int scoringPlayerNum = birdieTransform.position.x > 0 ? 1 : 2;
            OnPointScored(scoringPlayerNum);
        }
    }


    public void SetIgnoreBirdieCollision(bool ignoreCollision)
    {
        int racketLayer = LayerMask.NameToLayer("Racket");
        int birdieLayer = LayerMask.NameToLayer("Birdie");
        Physics.IgnoreLayerCollision(racketLayer, birdieLayer, ignoreCollision);
    }

}
