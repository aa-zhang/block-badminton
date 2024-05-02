using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MovingNetworkObject : NetworkBehaviour
{
    [SerializeField] private float positionErrorThreshold;
    [SerializeField] private bool isExtrapolationEnabled;
    [SerializeField] private bool isInterpolationEnabled;
    [SerializeField] private int lerpSpeed;


    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    private NetworkVariable<double> networkTime = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            networkPosition.Value = rb.position;
            networkVelocity.Value = rb.velocity;
            networkTime.Value = NetworkManager.Singleton.ServerTime.Time;
        }
        else
        {
            if (isExtrapolationEnabled)
            {
                rb.position = GetExtrapolatedPosition();
            }
            else if (isInterpolationEnabled)
            {
                rb.position = Vector3.Lerp(rb.position, networkPosition.Value, Time.deltaTime * lerpSpeed);
            }
            else
            {
                rb.position = networkPosition.Value;
            }
            rb.velocity = networkVelocity.Value;
        }
    }

    private Vector3 GetExtrapolatedPosition()
    {
        Vector3 estimatedPosition = networkPosition.Value + networkVelocity.Value * (float)(NetworkManager.Singleton.ServerTime.Time - networkTime.Value);

        Vector3 positionError = estimatedPosition - rb.position;
        if (positionError.magnitude > positionErrorThreshold)
        {
            return Vector3.Lerp(rb.position, estimatedPosition, Time.deltaTime * lerpSpeed);
        }
        else
        {
            return estimatedPosition;
        }
    }

    [Rpc(SendTo.Server)]
    private void SetNetworkVariablesRpc()
    {

    }
}
