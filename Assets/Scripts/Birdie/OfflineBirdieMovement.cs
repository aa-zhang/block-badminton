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

    public delegate void ClientHitDelayHandler(bool readEnabled);
    public static ClientHitDelayHandler OnSetReadEnabled;

    [SerializeField] private GameObject agent1;
    [SerializeField] private GameObject agent2;

    private GameEnvironmentManager gameEnv;

    // Start is called before the first frame update
    void Awake()
    {
        birdieRb = gameObject.GetComponent<Rigidbody>();
        birdieTransform = gameObject.transform;

        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();
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
    }

    private void OnDisable()
    {
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
    }

    public void SetBirdieGravityRpc(bool enableGravity)
    {
        this.enableGravity = enableGravity;
    }

    private void ApplyGravity()
    {
        birdieRb.AddForce(Constants.GRAVITY);
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

        enableGravity = true;
        birdieRb.velocity = forceVector;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            // Prevent players from hitting the birdie after it lands
            SetBirdieCollisionRpc(false);

            // Determine if player 1 or 2 should receive the point
            int scoringPlayerNum = birdieTransform.localPosition.x > 0 ? 1 : 2;
            if (gameEnv.isTraining)
            {
                agent1.GetComponent<PlayerAgent>().SetReward(scoringPlayerNum);
                agent2.GetComponent<PlayerAgent>().SetReward(scoringPlayerNum);
            }
            else
            {
                OnPointScored(scoringPlayerNum);
            }
        }
    }

    public void SetBirdieCollisionRpc(bool enableCollision)
    {
        this.enableCollision = enableCollision;
    }

}
