using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BirdieNetwork : NetworkBehaviour
{
    private NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    private NetworkVariable<Vector3> networkVelocity = new NetworkVariable<Vector3>(new Vector3(0, 0, 0), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    private Rigidbody birdieRb;
    // Start is called before the first frame update
    void Start()
    {
        birdieRb = GetComponent<Rigidbody>();   
    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            networkPosition.Value = transform.position;
            networkVelocity.Value = birdieRb.velocity; // TODO: will probably have to change this, or make birdie kinematic
        }
        else
        {
            transform.position = networkPosition.Value;
            birdieRb.velocity = networkVelocity.Value;
        }
    }
}
