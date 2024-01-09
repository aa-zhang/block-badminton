using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieMovement : MonoBehaviour
{
    private ScoreManager scoreManager;
    private Rigidbody birdieRb;
    private Transform birdieTransform;

    private Transform playerOneTransform;
    private Transform playerTwoTransform;

    private PlayerMovement playerOneMovement;
    private PlayerMovement playerTwoMovement;

    private int scoringPlayerNum;

    private Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    private Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);

    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        playerOneTransform = GameObject.Find("Player 1").transform;
        playerTwoTransform = GameObject.Find("Player 2").transform;
        playerOneMovement = GameObject.Find("Player 1").GetComponent<PlayerMovement>();
        playerTwoMovement = GameObject.Find("Player 2").GetComponent<PlayerMovement>();
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

            Invoke("StartNextServe", 1);
        }
    }


    private void StartNextServe()
    {
        if (scoringPlayerNum == 1)
        {
            playerOneMovement.SetIsServing(true);
        }
        else
        {
            playerTwoMovement.SetIsServing(true);
        }
    }

}
