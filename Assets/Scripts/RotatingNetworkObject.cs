using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotatingNetworkObject : NetworkBehaviour
{
    [SerializeField] private int lerpSpeed;

    private NetworkVariable<Quaternion> networkQuaternion;
    private Transform objTransform;
    // Start is called before the first frame update
    void Awake()
    {
        objTransform = gameObject.transform;

        networkQuaternion = new NetworkVariable<Quaternion>(objTransform.rotation, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    }

    // Update is called once per frame
    void Update()
    {
        if (IsOwner)
        {
            networkQuaternion.Value = objTransform.rotation;
        }
        else
        {
            objTransform.rotation = Quaternion.Lerp(objTransform.rotation, networkQuaternion.Value, Time.deltaTime * lerpSpeed);
        }
    }
}
