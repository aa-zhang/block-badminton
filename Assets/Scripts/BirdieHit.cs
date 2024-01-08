using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdieHit : MonoBehaviour
{
    private Transform racketTransform;
    private Rigidbody birdieRb;
    [SerializeField] private float racketForce = 1000f;
    [SerializeField] private float birdieAngleAdjustment = 0.1f;



    // Start is called before the first frame update
    void Start()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Racket"))
        {
            racketTransform = collider.gameObject.transform;
            SwingRacket swingRacket = collider.gameObject.GetComponent<SwingRacket>();
            GameObject player = racketTransform.parent.gameObject;

            float theta;
            if (player.GetComponent<PlayerMovement>().isPlayerOne)
            {
                theta = racketTransform.eulerAngles.z;
            }
            else
            {
                theta = 180 - racketTransform.eulerAngles.z;
            }

            if (swingRacket.inBackwardSwingAnimation)
            {
                theta -= 180;
            }

            theta = theta * Mathf.PI / 180;
            Vector3 forceVector = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta) + birdieAngleAdjustment, 0).normalized;

            if (!swingRacket.alreadyMadeContact && (swingRacket.inForwardSwingAnimation || swingRacket.inBackwardSwingAnimation))
            {
                birdieRb.velocity = Vector3.zero;
                birdieRb.AddForce(forceVector * racketForce);
                swingRacket.SetAlreadyMadeContact(true);
                Debug.Log("hit the bertholdt with force. ARE WE DOING IT REINIER?!?!?" + forceVector);
                Debug.Log("theta " + theta);
            }

        }

    }

}
