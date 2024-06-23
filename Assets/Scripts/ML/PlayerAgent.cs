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
            GameMenu.Instance.ResetGameValues(gameEnv.id);
        }
    }


    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(opponent.transform.localPosition);

        sensor.AddObservation(birdie.transform.localPosition);
        sensor.AddObservation(birdie.GetComponent<Rigidbody>().velocity);

        sensor.AddObservation(staminaManager.currentStamina);
        sensor.AddObservation(opponentStaminaManager.currentStamina);

        sensor.AddObservation(offlineServeController.isServing);
        sensor.AddObservation(offlineServeController.isOpponentServing);

        sensor.AddObservation(offlineServeController.GetServeAngle());
        sensor.AddObservation(opponentOfflineServeController.GetServeAngle());

    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int runAction = actions.DiscreteActions[0];
        int dashAction = actions.DiscreteActions[1];
        int jumpAction = actions.DiscreteActions[2];
        int swingAction = actions.DiscreteActions[3];
        int fastFallAction = actions.DiscreteActions[4];
        int changeServeAngleAction = actions.DiscreteActions[5];

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

        if (dashAction == 0)
        {
            playerMovement.DashLeft();
        }
        else if (dashAction == 1)
        {
            playerMovement.DashRight();
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
        }
        else if (swingAction == 1)
        {
            playerMovement.SwingRacket(SwingType.Light);
        }
        else
        {
            // Don't swing racket
        }

        if (fastFallAction == 0)
        {
            playerMovement.FastFall();
        }
        else
        {
            playerMovement.CancelFastFall();
        }

        if (offlineServeController.isServing)
        {
            if (changeServeAngleAction == 0)
            {
                offlineServeController.ChangeServeAngle();
            }
            else
            {
                // Don't change serve angle
            }
        }

        // Add time penalty to prevent taking too long to serve
        //if (offlineServeController.isServing)
        //{
        //    AddReward(-0.005f);
        //}
        //else
        //{
        //    AddReward(-0.001f);
        //}
    }

    //private void HitBirdie_OnBirdieHit(Vector3 force, int playerNum)
    //{
    //    if (playerManager.playerNum == playerNum)
    //    {
    //        AddReward(0.2f);
    //    }
    //}

    public void SetReward(int scoringPlayerNum)
    {
        if (playerManager.playerNum == scoringPlayerNum)
        {
            SetReward(1f);
        }
        else
        {
            SetReward(-1f);
        }
        EndEpisode();
    }
}
