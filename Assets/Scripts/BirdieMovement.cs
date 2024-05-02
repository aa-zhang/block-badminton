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
    [SerializeField] private int forceMultiplier;

    public delegate void IncreaseScoreHandler(int scoringPlayerNum);
    public static IncreaseScoreHandler OnPointScored;


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
        if (!enableCollision) return;

        enableGravity = true;
        birdieRb.velocity = forceVector * forceMultiplier;
        // birdieRb.AddForce(forceVector, ForceMode.Impulse);
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

    [Rpc(SendTo.Server)]
    public void SetBirdieCollisionRpc(bool enableCollision)
    {
        this.enableCollision = enableCollision;
    }

}
