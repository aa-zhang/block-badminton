using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class BirdieSync : MonoBehaviourPun, IPunObservable
{
    // Reference: https://www.sharpcoderblog.com/blog/sync-rigidbodies-over-network-using-pun-2

    Rigidbody birdieRb;

    Vector3 latestPos;
    Quaternion latestRot;
    Vector3 velocity;
    Vector3 angularVelocity;

    bool valuesReceived = false;

    // Start is called before the first frame update
    void Start()
    {
        birdieRb = GetComponent<Rigidbody>();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            //We own this player: send the others our data
            Debug.Log($"I am player {photonView.ViewID} and I am sending data");
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(birdieRb.velocity);
            stream.SendNext(birdieRb.angularVelocity);
        }
        else
        {
            //Network player, receive data
            Debug.Log($"I am player {photonView.ViewID} and I am receiving");
            latestPos = (Vector3)stream.ReceiveNext();
            latestRot = (Quaternion)stream.ReceiveNext();
            velocity = (Vector3)stream.ReceiveNext();
            angularVelocity = (Vector3)stream.ReceiveNext();

            valuesReceived = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && valuesReceived)
        {
            //Update Object position and Rigidbody parameters
            transform.position = Vector3.Lerp(transform.position, latestPos, Time.deltaTime * 5);
            transform.rotation = Quaternion.Lerp(transform.rotation, latestRot, Time.deltaTime * 5);
            birdieRb.velocity = velocity;
            birdieRb.angularVelocity = angularVelocity;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (photonView.IsMine)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Racket"))
            {
                //Transfer PhotonView of Rigidbody to our local player
                Player[] players = PhotonNetwork.PlayerList;
                foreach (Player player in players)
                {
                    if (!player.IsLocal)
                    {
                        photonView.TransferOwnership(player);
                        break;
                    }
                }
            }
        }
    }
}