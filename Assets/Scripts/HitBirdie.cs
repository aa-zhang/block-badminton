using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBirdie : MonoBehaviour
{
    private BirdieMovement birdieMovement;
    private Transform racketTransform;
    private Rigidbody birdieRb;
    private SwingRacket swingRacket;
    [SerializeField] private float racketForce = 300;
    [SerializeField] private float birdieAngleAdjustment = 0.1f;



    // Start is called before the first frame update
    void Start()
    {
        racketTransform = gameObject.transform;
        swingRacket = gameObject.GetComponent<SwingRacket>();
        birdieRb = GameObject.Find("Birdie").GetComponent<Rigidbody>();
        birdieMovement = GameObject.Find("Birdie").GetComponent<BirdieMovement>();
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
        if (collider.gameObject.layer == LayerMask.NameToLayer("Birdie"))
        {
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
            Vector3 forceVector = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
            forceVector = swingRacket.overhand ? forceVector : -forceVector;
            forceVector = new Vector3(forceVector.x, forceVector.y + birdieAngleAdjustment, forceVector.z).normalized;
            if (!swingRacket.alreadyMadeContact && swingRacket.inForwardSwingAnimation)
            {
                birdieRb.velocity = Vector3.zero;
                birdieRb.AddForce(forceVector * racketForce);
                swingRacket.SetAlreadyMadeContact(true);
                birdieMovement.setIsServing(false);
                Debug.Log("hit the bertholdt with force. ARE WE DOING IT REINIER?!?!?" + forceVector);
                Debug.Log("theta " + theta);
            }

        }

    }

}
