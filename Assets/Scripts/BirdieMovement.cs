using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieMovement : MonoBehaviour
{
    public ScoreManager scoreManager;

    public GameObject playerOne;
    public GameObject playerTwo;

    private Transform playerOneTransform;
    private Transform playerTwoTransform;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private Rigidbody birdieRb;
    private Transform birdieTransform;

    private int scoringPlayerNum;

    private Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    private Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);

    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;
        playerOneTransform = playerOne.transform;
        playerTwoTransform = playerTwo.transform;
        playerOneMovement = playerOne.GetComponent<PlayerMovement>();
        playerTwoMovement = playerTwo.GetComponent<PlayerMovement>();

        SetIgnoreBirdieCollision(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (playerOneMovement.isServing || playerTwoMovement.isServing)
        {
            FollowPlayer();
        }
        else
        {
            ApplyGravity();
        }
    }

    private void FollowPlayer()
    {
        if (playerOneMovement.isServing)
        {
            birdieTransform.position = playerOneTransform.position + servingOffsetOne;
        }
        else
        {
            birdieTransform.position = playerTwoTransform.position + servingOffsetTwo;
        }
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(new Vector3(0, -4, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            scoringPlayerNum = birdieTransform.position.x > 0 ? 1 : 2;
            scoreManager.IncreaseScore(scoringPlayerNum);

            // Prevent players from hitting the birdie after it lands
            SetIgnoreBirdieCollision(true);

            Invoke("StartNextServe", 1);
        }
    }


    private void StartNextServe()
    {
        // Re-enable the racket-birdie collision
        SetIgnoreBirdieCollision(false);

        if (scoringPlayerNum == 1)
        {
            playerOneMovement.SetIsServing(true);
        }
        else
        {
            playerTwoMovement.SetIsServing(true);
        }
    }

    private void SetIgnoreBirdieCollision(bool ignoreCollision)
    {
        int racketLayer = LayerMask.NameToLayer("Racket");
        int birdieLayer = LayerMask.NameToLayer("Birdie");
        Physics.IgnoreLayerCollision(racketLayer, birdieLayer, ignoreCollision);

    }

}
