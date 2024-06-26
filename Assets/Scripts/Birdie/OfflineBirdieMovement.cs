using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBirdieMovement : MonoBehaviour
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
        if (enableGravity)
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

    public void SetBirdieGravityRpc(bool enableGravity)
    {
        this.enableGravity = enableGravity;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(Constants.GRAVITY);
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum)
    {
        ApplyForceToBirdieRpc(forceVector, birdieRb.position, playerNum);
    }

    public void ApplyForceToBirdieRpc(Vector3 forceVector, Vector3 position, int playerNum)
    {
        if (!enableCollision) return;

        enableGravity = true;
        birdieRb.velocity = forceVector;
        birdieRb.position = position;
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

    public void SetBirdieCollisionRpc(bool enableCollision)
    {
        this.enableCollision = enableCollision;
    }

}
