using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBirdie : MonoBehaviour
{
    public GameObject birdie;
    private BirdieParticleController birdiePsController;

    private Transform racketTransform;
    private Rigidbody birdieRb;
    private SwingRacket swingRacket;
    private GameObject player;
    private PlayerMovement playerMovement;

    [SerializeField] private float racketForce = 300;
    [SerializeField] private float birdieAngleAdjustment = 0.1f;



    // Start is called before the first frame update
    void Start()
    {
        racketTransform = gameObject.transform;
        swingRacket = gameObject.GetComponent<SwingRacket>();
        birdieRb = birdie.GetComponent<Rigidbody>();
        player = racketTransform.parent.gameObject;
        playerMovement = player.GetComponent<PlayerMovement>();
        birdiePsController = birdie.GetComponent<BirdieParticleController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Birdie"))
        {
            float theta;
            if (playerMovement.isPlayerOne)
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

            // Convert degrees to radians
            theta = theta * Mathf.Deg2Rad;

            // Use math to calculate (x, y, z) given the theta
            Vector3 forceVector = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);

            // Need to use oppposite direction if hitting with underhand swing
            forceVector = swingRacket.overhand ? forceVector : -forceVector;

            // Add a slight angle adjustment and then normalize the vector
            forceVector = new Vector3(forceVector.x, forceVector.y + birdieAngleAdjustment, forceVector.z).normalized;

            if (swingRacket.inForwardSwingAnimation && !swingRacket.alreadyMadeContact)
            {
                // Racket is currently in the forward swinging animation
                // And is the first time making contact with the birdie during this swing

                birdieRb.velocity = Vector3.zero;
                birdieRb.AddForce(forceVector * racketForce, ForceMode.Impulse);
                swingRacket.SetAlreadyMadeContact(true);
                playerMovement.SetIsServing(false);
                Debug.Log("hit the bertholdt with force. ARE WE DOING IT REINIER?!?!?" + forceVector);
                Debug.Log("theta " + theta);
            }

        }

    }

}
