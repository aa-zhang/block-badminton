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

    private Vector3 servingOffsetOne = new Vector3(2, -0.7f, 0);
    private Vector3 servingOffsetTwo = new Vector3(-2, -0.7f, 0);


    public bool isServing = true;
    private int scoringPlayer = 1;
    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;
        scoreManager = GameObject.Find("ScoreManager").GetComponent<ScoreManager>();
        playerOneTransform = GameObject.Find("Player 1").transform;
        playerTwoTransform = GameObject.Find("Player 2").transform;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (isServing)
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
        if (scoringPlayer == 1)
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
        birdieRb.AddForce(new Vector3(0, -2, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            scoringPlayer = birdieTransform.position.x > 0 ? 1 : 2;
            scoreManager.IncreaseScore(scoringPlayer);

            isServing = true;
        }
    }

    public void setIsServing(bool isServing)
    {
        this.isServing = isServing;
    }
}
