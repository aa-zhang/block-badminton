using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BirdieMovement : NetworkBehaviour
{
    private Rigidbody birdieRb;
    private Transform birdieTransform;
    private bool enableGravity = false;
    private bool enableCollision = true;

    public delegate void IncreaseScoreHandler(int scoringPlayerNum);
    public static IncreaseScoreHandler OnPointScored;

    public delegate void ClientHitDelayHandler(bool readEnabled);
    public static ClientHitDelayHandler OnSetReadEnabled;


    // Start is called before the first frame update
    void Awake()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (enableGravity && IsServer)
        {
            ApplyGravity();
        }
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
    }

    [Rpc(SendTo.Server)]
    public void SetBirdieGravityRpc(bool enableGravity)
    {
        this.enableGravity = enableGravity;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(Constants.gravity);
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        if (!IsServer)
        {
            // Freeze birdie at position that it was hit
            birdieRb.velocity = Vector3.zero;
            OnSetReadEnabled(false);
        }


        // Let server handle birdie movement
        if (!enableCollision) return;
        ApplyForceToBirdieRpc(forceVector, birdieRb.position, playerNum);
    }

    [Rpc(SendTo.Server)]
    public void ApplyForceToBirdieRpc(Vector3 forceVector, Vector3 position, int playerNum)
    {
        enableGravity = true;
        birdieRb.velocity = forceVector;
        birdieRb.position = position;
        NotifyClientHitReceivedRpc();
    }

    [Rpc(SendTo.NotServer)]
    public void NotifyClientHitReceivedRpc()
    {
        // The server has received and set its values to the client's hit
        // Client can resume reading from server values
        OnSetReadEnabled(true);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            // Prevent players from hitting the birdie after it lands
            SetBirdieCollisionRpc(false);

            // Determine if player 1 or 2 should receive the point
            int scoringPlayerNum = birdieTransform.position.x > 0 ? 1 : 2;
            OnPointScored(scoringPlayerNum);
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SetBirdieCollisionRpc(bool enableCollision)
    {
        this.enableCollision = enableCollision;
    }

}
