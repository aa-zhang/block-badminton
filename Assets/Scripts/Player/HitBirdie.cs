using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBirdie : MonoBehaviour
{
    private Transform racketTransform;
    private SwingRacket swingRacket;
    private GameObject player;
    private PlayerManager playerManager;
    private PlayerMovement playerMovement;

    [SerializeField] private float racketForce = 300;
    [SerializeField] private float birdieAngleAdjustment = 0.1f;

    public delegate void BirdieHitHandler(Vector3 force, int playerNum);
    public static BirdieHitHandler OnBirdieHit;


    // Start is called before the first frame update
    void Start()
    {
        racketTransform = gameObject.transform;
        swingRacket = gameObject.GetComponent<SwingRacket>();
        player = racketTransform.parent.gameObject;
        playerManager = player.GetComponent<PlayerManager>();
        playerMovement = player.GetComponent<PlayerMovement>();
    }


    public void CalculateBirdieForce(bool isOverhandCollider)
    {
        // Don't register collisions for the incorrect collider
        if ((swingRacket.overhand && !isOverhandCollider) || (!swingRacket.overhand && isOverhandCollider))
        {
            return;
        }

        // Get racket angle
        float theta;
        if (playerManager.playerNum == 1)
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

        // Calculate (x, y, z) vector given the theta
        Vector3 forceVector = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);

        // Need to use oppposite direction if hitting with underhand swing
        forceVector = swingRacket.overhand ? forceVector : -forceVector;

        // Add a slight angle adjustment and then normalize the vector
        forceVector = new Vector3(forceVector.x, forceVector.y + birdieAngleAdjustment, forceVector.z).normalized;

        if (swingRacket.inForwardSwingAnimation && !swingRacket.alreadyMadeContact)
        {
            // Racket is currently in the forward swinging animation
            // And is the first time making contact with the birdie during this swing
            OnBirdieHit(forceVector * racketForce, playerManager.playerNum);
            swingRacket.SetAlreadyMadeContact(true);
            Debug.Log("hit the bertholdt with force. ARE WE DOING IT REINIER?!?!?" + forceVector);
            Debug.Log("theta " + theta);
        }

    }

    

}
