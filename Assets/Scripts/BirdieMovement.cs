using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BirdieMovement : NetworkBehaviour
{
    private Rigidbody birdieRb;
    private Transform birdieTransform;
    private bool enableGravity = false;

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

    public void SetEnableBirdieGravity(bool enableGravity)
    {
        this.enableGravity = enableGravity;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(new Vector3(0, -4, 0));
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        // Let server handle birdie movement
        ApplyForceToBirdieRpc(forceVector, playerNum);
    }

    [Rpc(SendTo.Server)]
    public void ApplyForceToBirdieRpc(Vector3 forceVector, int playerNum)
    {
        if (!enableGravity)
        {
            enableGravity = true;
        }
        birdieRb.velocity = Vector3.zero;
        birdieRb.AddForce(forceVector, ForceMode.Impulse);
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            // Prevent players from hitting the birdie after it lands
            Debug.LogError("disable collision"); // TODO: client collision not being disabled
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
