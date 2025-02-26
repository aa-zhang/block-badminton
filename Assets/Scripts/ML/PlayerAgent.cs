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
    private OfflineServeController opponentOfflineServeController;
    private PlayerMovement playerMovement;
    private PlayerManager playerManager;
    private StaminaManager staminaManager;
    private StaminaManager opponentStaminaManager;


    private GameEnvironmentManager gameEnv;

    private void Start()
    {
        playerMovement = GetComponent<PlayerMovement>();
        playerManager = GetComponent<PlayerManager>();
        offlineServeController = GetComponent<OfflineServeController>();
        opponentOfflineServeController = opponent.GetComponent<OfflineServeController>();
        staminaManager = GetComponent<StaminaManager>();
        opponentStaminaManager = opponent.GetComponent<StaminaManager>();

        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
    }


    public override void OnEpisodeBegin()
    {
        if (playerManager.playerNum == 1)
        {
            Debug.Log(gameEnv.id);
            offlineGameStateManager.RestartGameRpc();
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(opponent.transform.localPosition);

        sensor.AddObservation(birdie.transform.localPosition);
        sensor.AddObservation(birdie.GetComponent<Rigidbody>().velocity);

        sensor.AddObservation(offlineServeController.isServing);
        sensor.AddObservation(offlineServeController.isOpponentServing);

        sensor.AddObservation(offlineServeController.GetServeAngle());
        // sensor.AddObservation(opponentOfflineServeController.GetServeAngle());

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int runAction = actions.DiscreteActions[0];
        int jumpAction = actions.DiscreteActions[1];
        int swingAction = actions.DiscreteActions[2];
        int changeServeAngleAction = actions.DiscreteActions[3];

        if (runAction == 0)
        {
            playerMovement.MoveLeft();
        }
        else if (runAction == 1)
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
            playerMovement.SwingRacket(SwingType.Heavy);
            AddReward(-0.2f);
        }
        else
        {
            // Don't swing racket
        }


        if (offlineServeController.isServing)
        {
            if (changeServeAngleAction == 0)
            {
                offlineServeController.ChangeServeAngle();
                AddReward(-0.05f);
            }
            else
            {
                // Don't change serve angle
            }
        }

        // Add time penalty to prevent taking too long to serve
        if (offlineServeController.isServing)
        {
            AddReward(-0.005f);
        }
        //else
        //{
        //    AddReward(-0.001f);
        //}
    }

    private void HitBirdie_OnBirdieHit(Vector3 force, int playerNum)
    {
        if (playerManager.playerNum == playerNum)
        {
            AddReward(0.2f);
        }
    }

    public void SetReward(int scoringPlayerNum)
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
    }
}
