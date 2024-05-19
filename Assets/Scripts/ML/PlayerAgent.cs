using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class PlayerAgent : Agent
{
    [SerializeField] private GameObject birdie;
    [SerializeField] private GameObject opponent;
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;


    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = GetComponent<PlayerManager>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        OfflineBirdieMovement.OnPointScored += BirdieMovement_OnPointScored;
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
    }

    protected override void OnDisable()
    {
        base.OnEnable();
        OfflineBirdieMovement.OnPointScored -= BirdieMovement_OnPointScored;
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
    }

    public override void OnEpisodeBegin()
    {
        OfflineGameStateManager.Instance.RestartGameRpc();
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.position);
        sensor.AddObservation(opponent.transform.position);

        sensor.AddObservation(birdie.transform.position);
        sensor.AddObservation(birdie.GetComponent<Rigidbody>().velocity);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int horizontalMovement = actions.DiscreteActions[0];
        int jumpAction = actions.DiscreteActions[1];
        int swingAction = actions.DiscreteActions[2];

        if (horizontalMovement == 0)
        {
            playerMovement.MoveLeft();
        }
        else if (horizontalMovement == 1)
        {
            playerMovement.MoveRight();
        }
        else
        {
            // Don't move
        }

        if (jumpAction == 0)
        {
            playerMovement.Jump();
        }
        else
        {
            // Don't jump
        }

        if (swingAction == 0)
        {
            playerMovement.SwingRacket();
        }
        else
        {
            // Don't swing racket
        }
    }

    private void HitBirdie_OnBirdieHit(Vector3 force, int playerNum)
    {
        if (playerManager.playerNum == playerNum)
        {
            SetReward(0.2f);
        }
    }

    private void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (playerManager.playerNum == scoringPlayerNum)
        {
            SetReward(+1f);
        }
        else
        {
            SetReward(-1f);
        }
        EndEpisode();
        Debug.Log("episode end");
    }
}
