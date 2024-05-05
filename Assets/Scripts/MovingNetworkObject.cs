using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingNetworkObject : NetworkBehaviour
{
    [SerializeField] private float positionErrorThreshold;
    [SerializeField] private bool isExtrapolationEnabled;
    [SerializeField] private bool isInterpolationEnabled;
    [SerializeField] private bool isPredictionEnabled;

    [SerializeField] private int lerpSpeed;
    [SerializeField] private int predictionSteps;


    private bool readEnabled = true;
    private NetworkVariable<Vector3> networkPosition;
    private NetworkVariable<Vector3> networkVelocity;
    private NetworkVariable<double> networkTime;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;

        networkPosition = new NetworkVariable<Vector3>(rb.position, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        networkVelocity = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        networkTime = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    }


    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            SetNetworkVariables();
        }
        else
        {
            if (readEnabled)
            {
                ReadNetworkVariables();
            }
        }
    }

    private void SetNetworkVariables()
    {
        networkPosition.Value = rb.position;
        networkVelocity.Value = rb.velocity;
        networkTime.Value = NetworkManager.Singleton.ServerTime.Time;
    }

    private void ReadNetworkVariables()
    {
        if (isExtrapolationEnabled)
        {
            rb.position = GetExtrapolatedPosition();
        }
        else if (isInterpolationEnabled)
        {
            rb.position = Vector3.Lerp(rb.position, networkPosition.Value, Time.deltaTime * lerpSpeed);
        }
        else if (isPredictionEnabled)
        {
            rb.position = GetPredictedTrajectoryPosition(predictionSteps);
        }
        else
        {
            rb.position = networkPosition.Value;
        }
        rb.velocity = networkVelocity.Value;
    }

    private Vector3 GetExtrapolatedPosition()
    {
        Vector3 estimatedPos = networkPosition.Value + networkVelocity.Value * (float)(NetworkManager.Singleton.ServerTime.Time - networkTime.Value);

        Vector3 positionError = estimatedPos - rb.position;
        if (positionError.magnitude > positionErrorThreshold)
        {
            return Vector3.Lerp(rb.position, estimatedPos, Time.deltaTime * lerpSpeed);
        }
        else
        {
            return estimatedPos;
        }
    }

    private Vector3 GetPredictedTrajectoryPosition(int steps)
    {
        Vector3 predictedPos = networkPosition.Value;
        float timestep = Time.fixedDeltaTime / Physics.defaultSolverVelocityIterations;
        Vector3 gravityAccel = Constants.gravity * timestep * timestep;
        float drag = 1f - timestep * rb.drag;
        Vector3 moveStep = networkVelocity.Value * timestep;

        // Iterate to get the {steps}th predicted position in the trajectory
        for (int i = 0; i < steps; i++)
        {
            moveStep += gravityAccel;
            moveStep *= drag;
            predictedPos += moveStep;
        }

        return Vector3.Lerp(rb.position, predictedPos, Time.deltaTime * lerpSpeed); ;
    }


    private void OnEnable()
    {
        BirdieMovement.OnSetReadEnabled += BirdieMovement_OnSetReadEnabled;
    }

    private void OnDisable()
    {
        BirdieMovement.OnSetReadEnabled -= BirdieMovement_OnSetReadEnabled;
    }

    private void BirdieMovement_OnSetReadEnabled(bool readEnabled)
    {
        this.readEnabled = readEnabled;
    }
}
