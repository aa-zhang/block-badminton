using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieMovement : MonoBehaviour
{
    private Rigidbody birdieRb;
    private Transform birdieTransform;

    private Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    private Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);

    public delegate void IncreaseScoreHandler(int scoringPlayerNum);
    public static IncreaseScoreHandler OnPointScored;

    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;

        SetIgnoreBirdieCollision(false);
    }

    private void FixedUpdate()
    {
        ApplyGravity();
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector)
    {
        // Apply a force onto the birdie
        birdieRb.velocity = Vector3.zero;
        birdieRb.AddForce(forceVector, ForceMode.Impulse);
    }

    public void FollowPlayer(Vector3 playerPosition, bool isPlayerOne)
    {
        Vector3 servingOffset = isPlayerOne ? servingOffsetOne : servingOffsetTwo;
        birdieTransform.position = playerPosition + servingOffset;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(new Vector3(0, -4, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            // Determine if player 1 or 2 should receive the point
            int scoringPlayerNum = birdieTransform.position.x > 0 ? 1 : 2;
            OnPointScored(scoringPlayerNum);

            // Prevent players from hitting the birdie after it lands
            SetIgnoreBirdieCollision(true);
        }
    }


    public void SetIgnoreBirdieCollision(bool ignoreCollision)
    {
        int racketLayer = LayerMask.NameToLayer("Racket");
        int birdieLayer = LayerMask.NameToLayer("Birdie");
        Physics.IgnoreLayerCollision(racketLayer, birdieLayer, ignoreCollision);

    }

}
