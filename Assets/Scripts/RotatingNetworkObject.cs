using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class RotatingNetworkObject : NetworkBehaviour
{
    [SerializeField] private int lerpSpeed;

    private NetworkVariable<Quaternion> networkQuaternion = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private Transform objTransform;
    // Start is called before the first frame update
    void Start()
    {
        objTransform = gameObject.transform;
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
