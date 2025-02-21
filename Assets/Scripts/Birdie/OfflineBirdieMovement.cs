using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineBirdieMovement : MonoBehaviour
{
    private Rigidbody birdieRb;
    private Transform birdieTransform;
    private bool enableGravity = false;
    private bool enableCollision = true;

    public delegate void IncreaseScoreHandler(int scoringPlayerNum);
    public static IncreaseScoreHandler OnPointScored;
    private bool pointAlreadyScored = false;

    public delegate void ClientHitDelayHandler(bool readEnabled);
    public static ClientHitDelayHandler OnSetReadEnabled;

    [SerializeField] private GameObject agent1;
    [SerializeField] private GameObject agent2;

    private GameEnvironmentManager gameEnv;
    private BirdieParticleController birdiePsController;


    // Start is called before the first frame update
    void Awake()
    {
        birdieRb = GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;

        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
        birdiePsController = GetComponent<BirdieParticleController>();
    }

    private void FixedUpdate()
    {
        if (enableGravity)
        {
            ApplyGravity();
        }
    }

    private void OnEnable()
    {
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
        OfflineGameStateManager.OnBeginServe += OfflineGameStateManager_OnBeginServe;
        OfflineGameStateManager.OnGameStateChange += OfflineGameStateManager_OnGameStateChange;
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        OfflineGameStateManager.OnBeginServe -= OfflineGameStateManager_OnBeginServe;
        OfflineGameStateManager.OnGameStateChange -= OfflineGameStateManager_OnGameStateChange;
    }

    public void SetBirdieGravityRpc(bool enableGravity)
    {
        this.enableGravity = enableGravity;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(new Vector3(0, Constants.GRAVITY * birdieRb.mass, 0));
    }

    private void OfflineGameStateManager_OnBeginServe(int servingPlayerNum, int gameEnv)
    {
        if (this.gameEnv.id != gameEnv)
        {
            return;
        }
        SetBirdieGravityRpc(false);
        SetBirdieCollisionRpc(true);
        ResetVelocities();
    }

    private void OfflineGameStateManager_OnGameStateChange(GameState gameState)
    {
        if (gameState == GameState.NotPlaying)
        {
            birdiePsController.EnableTrailRenderer(false);
            SetBirdieGravityRpc(false);
            SetBirdieCollisionRpc(true);
            ResetVelocities();
            ResetPosition();
        }
        if (gameState == GameState.Serving)
        {
            birdiePsController.EnableTrailRenderer(true);
        }
        
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum, int gameEnvId)
    {
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }
        ApplyForceToBirdieRpc(forceVector, birdieTransform.localPosition, playerNum);
    }

    public void ApplyForceToBirdieRpc(Vector3 forceVector, Vector3 position, int playerNum)
    {
        if (!enableCollision) return;
        pointAlreadyScored = false;
        enableGravity = true;
        birdieRb.velocity = forceVector;
        int rotationDirection = playerNum == 1 ? 1 : -1;
        float rotationSpeed = Mathf.Abs(forceVector.y) / 3;
        birdieRb.angularVelocity = Vector3.forward * rotationDirection * rotationSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if ((collision.gameObject.layer == LayerMask.NameToLayer("Floor") || collision.gameObject.layer == LayerMask.NameToLayer("Outside Floor")) && !pointAlreadyScored)
        {
            // Prevent players from hitting the birdie after it lands
            SetBirdieCollisionRpc(false);

            // Determine if player 1 or 2 should receive the point
            int scoringPlayerNum;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
            {
                // Birdie landed within the court
                scoringPlayerNum = birdieTransform.localPosition.x > 0 ? 1 : 2;
            }
            else
            {
                // Birdie landed out of bounds
                scoringPlayerNum = birdieTransform.localPosition.x < 0 ? 1 : 2;
            }

            if (gameEnv.isTraining)
            {
                agent1.GetComponent<PlayerAgent>().SetReward(scoringPlayerNum);
                agent2.GetComponent<PlayerAgent>().SetReward(scoringPlayerNum);
            }
            else
            {
                OnPointScored(scoringPlayerNum);
                pointAlreadyScored = true;
            }
        }
    }

    public void SetBirdieCollisionRpc(bool enableCollision)
    {
        this.enableCollision = enableCollision;
    }

    public void ResetVelocities()
    {
        birdieRb.velocity = Vector3.zero;
        birdieRb.angularVelocity = Vector3.zero;
    }

    private void ResetPosition()
    {
        birdieTransform.position = Constants.BIRDIE_DEFAULT_POSITION;
    }

    

}
