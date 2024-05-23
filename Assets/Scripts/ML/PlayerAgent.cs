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

    [SerializeField] private OfflineGameStateManager offlineGameStateManager;
    private OfflineServeController offlineServeController;
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;

    [SerializeField] private int trainingEnvId;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = GetComponent<PlayerManager>();
        offlineServeController = GetComponent<OfflineServeController>();

    }


    public override void OnEpisodeBegin()
    {
        if (playerManager.playerNum == 1)
        {
            GameMenu.Instance.ResetGameValues(trainingEnvId);
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(opponent.transform.localPosition);

        sensor.AddObservation(birdie.transform.localPosition);
        sensor.AddObservation(birdie.GetComponent<Rigidbody>().velocity);

        sensor.AddObservation(offlineServeController.isServing);
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

        // Add time penalty to prevent taking too long to serve
        if (offlineServeController.isServing)
        {
            AddReward(-0.005f);
        }
        else
        {
            AddReward(-0.001f);
        }
    }

    //private void HitBirdie_OnBirdieHit(Vector3 force, int playerNum)
    //{
    //    if (playerManager.playerNum == playerNum)
    //    {
    //        AddReward(0.2f);
    //    }
    //}

    public void BirdieMovement_OnPointScored(int scoringPlayerNum)
    {
        if (playerManager.playerNum == scoringPlayerNum)
        {
            AddReward(1f);
        }
        else
        {
            AddReward(-1f);
        }
        EndEpisode();
        Debug.Log("episode end");
    }
}
