using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieMovement : MonoBehaviour
{

    private Rigidbody birdieRb;
    public bool isServing = true;
    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void Update()
    {
        if (isServing)
        {
            //FollowPlayer();
        }
        else
        {
            ApplyGravity();
        }
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(new Vector3(0, -1, 0));
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            Invoke("RespawnBirdie", 1);
            Debug.Log("touched grass");
        }
    }

    private void RespawnBirdie()
    {
        gameObject.transform.position = new Vector3(-8.46f, 6.4f, 0);
        gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
    }
}
