using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfflineServeController : MonoBehaviour, IServing
{
    public bool isServing { get; set; }
    public bool isOpponentServing { get; set; }

    private PlayerManager playerManager;
    private Transform playerTransform;
    private Rigidbody playerRb;
    private PlayerMovement playerMovement;

    private Transform birdieTransform;
    private OfflineBirdieMovement birdieMovement;
    private TrailRenderer birdieTrailRenderer;

    [SerializeField] private GameObject serveArrow;
    [SerializeField] private float lerpDuration = 0.1f;

    private ServeAngle currentServeAngle = ServeAngle.High;

    public delegate void HitServeHandler();
    public static HitServeHandler OnHitServe;

    private GameEnvironmentManager gameEnv;


    // Start is called before the first frame update
    void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        playerTransform = gameObject.transform;
        playerRb = GetComponent<Rigidbody>();
        gameEnv = transform.root.GetComponent<GameEnvironmentManager>();

    }

    private void FixedUpdate()
    {
        if (isServing)
        {
            HoldBirdieRpc();
        }
    }

    private void OnEnable()
    {
        OfflineGameStateManager.OnBirdieInitialized += GameStateManager_OnBirdieInitialized;
        OfflineGameStateManager.OnBeginServe += GameStateManager_OnBeginServe;
        HitBirdie.OnBirdieHit += HitBirdie_OnBirdieHit;
        GameMenu.OnGameRestart += GameMenu_OnGameRestart;
    }

    private void OnDisable()
    {
        OfflineGameStateManager.OnBirdieInitialized -= GameStateManager_OnBirdieInitialized;
        OfflineGameStateManager.OnBeginServe -= GameStateManager_OnBeginServe;
        HitBirdie.OnBirdieHit -= HitBirdie_OnBirdieHit;
        GameMenu.OnGameRestart -= GameMenu_OnGameRestart;
    }

    private void HoldBirdieRpc()
    {
        // Move the birdie in front of the serving player
        Vector3 servingOffset = playerManager.playerNum == 1 ? Constants.SERVING_OFFSET_PLAYER_ONE : Constants.SERVING_OFFSET_PLAYER_TWO;
        birdieTransform.localPosition = playerTransform.localPosition + servingOffset;
        birdieTransform.rotation = Quaternion.identity;

        // Hide trail
        birdieTrailRenderer.Clear();
    }

    private void GameStateManager_OnBirdieInitialized(GameObject birdie, int gameEnvId)
    {
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }
        // Attach spawned birdie to this script
        birdieTransform = birdie.transform;
        birdieMovement = birdie.GetComponent<OfflineBirdieMovement>();
        birdieTrailRenderer = birdie.GetComponent<TrailRenderer>();
    }

    private void GameStateManager_OnBeginServe(int playerNum, int gameEnvId)
    {
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }

        // Initiate serving sequence for given playerNum
        if (playerManager.playerNum == playerNum)
        {
            isServing = true;
            isOpponentServing = false;
            serveArrow.SetActive(true);

            birdieMovement.SetBirdieGravityRpc(false);
            birdieMovement.SetBirdieCollisionRpc(true);
            birdieMovement.ResetVelocities();
        }
        else
        {
            isServing = false;
            isOpponentServing = true;
            serveArrow.SetActive(false);

        }
        ResetPlayerPosition();

    }


    private void ResetPlayerPosition()
    {
        // Move player to serve position
        float newXPos;
        if (playerManager.playerNum == 1)
        {
            newXPos = -Constants.SERVE_X_POS;
        }
        else
        {
            newXPos = Constants.SERVE_X_POS;
        }

        playerTransform.localPosition = new Vector3(newXPos, Constants.GROUND_Y_POS, playerTransform.localPosition.z);
        playerRb.velocity = Vector3.zero;
        playerMovement.ResetJumpVariables();
    }

    private void HitBirdie_OnBirdieHit(Vector3 forceVector, int playerNum, int gameEnvId)
    {
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }

        if (isServing)
        {
            OnHitServe?.Invoke();
        }

        isServing = false;
        isOpponentServing = false;
        serveArrow.SetActive(false);

    }

    private void GameMenu_OnGameRestart(int gameEnvId)
    {
        if (gameEnv.isTraining && gameEnv.id != gameEnvId)
        {
            return;
        }
        ResetPlayerPosition();
    }

    public void ChangeServeAngle()
    {
        Vector3 targetRotation;

        if (currentServeAngle == ServeAngle.High)
        {
            targetRotation = Constants.SERVE_ANGLE_LOW;
            currentServeAngle = ServeAngle.Low;
        }
        else
        {
            targetRotation = Constants.SERVE_ANGLE_HIGH;
            currentServeAngle = ServeAngle.High;
        }
        StartCoroutine(LerpServeArrow(Quaternion.Euler(targetRotation), lerpDuration));
    }

    IEnumerator LerpServeArrow(Quaternion endValue, float duration)
    {
        float time = 0;
        Quaternion startValue = serveArrow.transform.localRotation;

        while (time < duration)
        {
            serveArrow.transform.localRotation = Quaternion.Lerp(startValue, endValue, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        serveArrow.transform.localRotation = endValue;
    }

    public Vector3 GetServeAngle()
    {
        return serveArrow.transform.localEulerAngles;
    }
}
