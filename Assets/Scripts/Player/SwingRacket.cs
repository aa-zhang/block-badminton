using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwingType
{
    Heavy,
    Light
}

public class SwingRacket : MonoBehaviour
{
    private Transform racketTransform;
    private Transform birdieTransform;
    private BirdieParticleController birdiePsController;

    private GameObject player;
    private PlayerManager playerManager;
    private Transform playerTransform;
    private PlayerMovement playerMovement;


    public bool alreadyMadeContact = false;
    public bool overhand = false;
    public SwingType swingType;

    // Animation variables
    public bool inForwardSwingAnimation = false;
    public bool inBackwardSwingAnimation = false;
    [SerializeField] private float racketSpeed = 0.09f;
    private float racketTimer = 0f;

    private Vector3 defaultAngle = new Vector3(0, 0, 55);
    private float endAngle;

    private float underhandDetectHeight = -0.5f;

    private GameEnvironmentManager gameEnv;


    // Start is called before the first frame update
    void Awake()
    {
        player = transform.parent.gameObject;
        playerTransform = player.transform;
        playerManager = player.GetComponent<PlayerManager>();
        playerMovement = player.GetComponent<PlayerMovement>();

        racketTransform = gameObject.transform;

        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
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
        GameStateManager.OnBirdieInitialized += GameStateManager_OnBirdieInitialized;
        OfflineGameStateManager.OnBirdieInitialized += OfflineGameStateManager_OnBirdieInitialized;

    }

    private void OnDisable()
    {
        GameStateManager.OnBirdieInitialized -= GameStateManager_OnBirdieInitialized;
        OfflineGameStateManager.OnBirdieInitialized += OfflineGameStateManager_OnBirdieInitialized;

    }

    private void GameStateManager_OnBirdieInitialized(GameObject birdie)
    {
        birdieTransform = birdie.transform;
    }

    private void OfflineGameStateManager_OnBirdieInitialized(GameObject birdie, int gameEnvId)
    {
        if (gameEnv.id != gameEnvId)
        {
            return;
        }
        birdieTransform = birdie.transform;
    }


    public void Swing(SwingType swingType)
    {
        // Begin the forward swing animation
        inForwardSwingAnimation = true;

        // Set swing type
        this.swingType = swingType;

        // Check the birdie position to determine how the player should swing
        if ((playerTransform.localPosition.y - birdieTransform.localPosition.y > underhandDetectHeight) &&
            ((playerManager.playerNum == 1 && birdieTransform.localPosition.x < 0) ||
            (playerManager.playerNum == 2 && birdieTransform.localPosition.x > 0)))
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
