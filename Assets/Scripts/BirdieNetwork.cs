using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BirdieNetwork : NetworkBehaviour
{
    [SerializeField] private float positionErrorThreshold;
    [SerializeField] private bool isExtrapolationEnabled;
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<double> networkTime = new NetworkVariable<double>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Rigidbody birdieRb;
    // Start is called before the first frame update
    void Start()
    {
        birdieRb = GetComponent<Rigidbody>();
        birdieRb.isKinematic = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsServer)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = birdieRb.velocity;
            networkTime.Value = NetworkManager.Singleton.ServerTime.Time;
        }
        else
        {
            if (isExtrapolationEnabled)
            {
                Vector3 estimatedPosition = networkPosition.Value + networkVelocity.Value * (float)(NetworkManager.Singleton.ServerTime.Time - networkTime.Value);

                Vector3 positionError = estimatedPosition - birdieRb.position;
                if (positionError.magnitude > positionErrorThreshold)
                {
                    birdieRb.position = Vector3.Lerp(birdieRb.position, estimatedPosition, Time.deltaTime * 10);
                }

                birdieRb.transform.forward = Vector3.Lerp(birdieRb.transform.forward, networkVelocity.Value.normalized, Time.deltaTime * 10);

                //transform.position = networkPosition.Value;
                birdieRb.velocity = networkVelocity.Value;
            }
            else
            {
                transform.position = networkPosition.Value;
                birdieRb.velocity = networkVelocity.Value;
            }
        }
    }
}
