using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwingRacket : MonoBehaviour
{
    private Transform racketTransform;

    public GameObject birdie;
    private Transform birdieTransform;
    private BirdieParticleController birdiePsController;

    private GameObject player;
    private PlayerManager playerManager;
    private Transform playerTransform;
    private PlayerMovement playerMovement;


    public bool alreadyMadeContact = false;
    public bool overhand = false;

    // Animation variables
    public bool inForwardSwingAnimation = false;
    public bool inBackwardSwingAnimation = false;
    [SerializeField] private float racketSpeed = 0.09f;
    private float racketTimer = 0f;

    private Vector3 defaultAngle = new Vector3(0, 0, 55);
    private float endAngle;



    // Start is called before the first frame update
    void Start()
    {
        player = transform.parent.gameObject;
        playerTransform = player.transform;
        playerManager = player.GetComponent<PlayerManager>();
        playerMovement = player.GetComponent<PlayerMovement>();

        racketTransform = gameObject.transform;
    }

    private void FixedUpdate()
    {
        if (inForwardSwingAnimation || inBackwardSwingAnimation)
        {
            PerformSwingAnimation();
        }
    }

    private void OnEnable()
    {
        GameStateManager.OnBirdieInitialized += ScoreManager_OnBirdieInitialized;
    }

    private void OnDisable()
    {
        GameStateManager.OnBirdieInitialized -= ScoreManager_OnBirdieInitialized;
    }

    private void ScoreManager_OnBirdieInitialized(GameObject birdie)
    {
        birdieTransform = birdie.transform;
    }


    public void Swing()
    {
        // Begin the forward swing animation
        inForwardSwingAnimation = true;

        // Check the birdie position to determine how the player should swing
        if ((playerTransform.position.y - birdieTransform.position.y > 0) &&
            ((playerManager.playerNum == 1 && birdieTransform.position.x < 0) ||
            (playerManager.playerNum == 2 && birdieTransform.position.x > 0)))
        {
            overhand = false;
        }
        else
        {
            overhand = true;
        }
    }

    private void PerformSwingAnimation()
    {
        // This method is called on each FixedUpdate iteration to set the next frame of the swing animation

        // Calculate the lerped angle of the racket
        float lerpedAngle = 0;
        if (inForwardSwingAnimation)
        {
            endAngle = overhand ? -95 : 265;

            lerpedAngle = Mathf.Lerp(defaultAngle.z, endAngle, racketTimer);
        }
        else if (inBackwardSwingAnimation)
        {
            lerpedAngle = Mathf.Lerp(endAngle, defaultAngle.z, racketTimer);
        }

        // Update the racket angle
        racketTransform.localEulerAngles = new Vector3(0, 0, lerpedAngle);

        // Increment the timer
        racketTimer += racketSpeed;

        // Check racketTimer to determine the stage of the swing animation
        if (racketTimer >= 1)
        {
            if (inForwardSwingAnimation)
            {
                // Completed forward swing, now starting backward swing
                inForwardSwingAnimation = false;
                inBackwardSwingAnimation = true;
            }
            else if (inBackwardSwingAnimation)
            {
                // Completed backward swing, stopping swing animation now
                inBackwardSwingAnimation = false;
                playerMovement.SetCanSwing(true);
                alreadyMadeContact = false;
                racketTransform.localEulerAngles = defaultAngle;
            }

            racketTimer = 0f;
        }

    }


    public void SetAlreadyMadeContact(bool madeContact)
    {
        alreadyMadeContact = madeContact;
    }


}
