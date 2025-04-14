using System;
using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{
    private PlayerManager playerManager;
    private Transform playerTransform;
    private Rigidbody playerRb;
    public GameObject racket;
    private SwingRacket swingRacket;
    private IServing serveController;
    private StaminaManager staminaManager;
    private DashTrailController dashTrailController;
    private Animator animator;

    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float jumpHeight = 800f;
    [SerializeField] private float dashSpeed = 35f;

    [SerializeField] private bool dashEnabled = false;
    [SerializeField] private bool fastFallEnabled = false;
    [SerializeField] private bool moveWhenServingEnabled = true;

    public static Action OnSwing;
    public static Action OnJump;


    private bool canSwing = true;
    private bool canFastFall = false;
    private bool isFastFalling = false;
    private bool failedFastFalling = false;
    private float fastFallTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerTransform = gameObject.transform;
        playerRb = GetComponent<Rigidbody>();
        swingRacket = racket.GetComponent<SwingRacket>();
        staminaManager = GetComponent<StaminaManager>();
        dashTrailController = GetComponent<DashTrailController>();
        serveController = GetComponent<OfflineServeController>();
        animator = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        DetectJumpApex();
        ApplyGravity();
        ApplyDrag();
        ClampPlayerPosition();
    }

    private void ApplyGravity()
    {
        if (!IsGrounded())
        {
            playerRb.AddForce(new Vector3(0, Constants.GRAVITY * playerRb.mass, 0));
        }
    }

    private void ApplyDrag()
    {
        // Need to add custom drag for the dash mechanic
        // Can't use built-in dash since it will also affect jump physics
        float newHorizontalVelocity = playerRb.velocity.x * (1 - Time.fixedDeltaTime * Constants.PLAYER_X_DRAG);
        playerRb.velocity = new Vector3(newHorizontalVelocity, playerRb.velocity.y, 0f);

    }

    private void ClampPlayerPosition()
    {
        Vector3 clampedPosition = transform.localPosition;
        if (playerManager.playerNum == 1)
        {
            if (serveController.isOpponentServing)
            {
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, -Constants.REAR_COURT_X_POS, -Constants.SERVING_LINE_X_POS);
            }
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, -Constants.REAR_COURT_X_POS, -Constants.FRONT_COURT_X_POS);
        }
        else
        {
            if (serveController.isOpponentServing)
            {
                clampedPosition.x = Mathf.Clamp(clampedPosition.x, Constants.SERVING_LINE_X_POS, Constants.REAR_COURT_X_POS);
            }
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, Constants.FRONT_COURT_X_POS, Constants.REAR_COURT_X_POS);
        }
        playerTransform.localPosition = clampedPosition;
    }

    private void DetectJumpApex()
    {
        // Detect the apex of the jump
        if (!IsGrounded() && playerRb.velocity.y <= 0 && !failedFastFalling)
        {
            canFastFall = true;
            fastFallTimer = 0.0f;
        }

        // Count down the apex time window
        if (canFastFall && !isFastFalling)
        {
            fastFallTimer += Time.deltaTime;
            if (fastFallTimer > Constants.FAST_FALL_TIME_FRAME)
            {
                canFastFall = false;
            }
        }
    }


    public void MoveLeft()
    {
        if (CanGoLeft())
        {
            playerTransform.localPosition += Vector3.left * movementSpeed * Time.deltaTime;
            animator.SetBool("IsMovingBackward", true);
            animator.SetBool("IsMovingForward", false);

        }
    }

    public void MoveRight()
    {
        if (CanGoRight())
        {
            playerTransform.localPosition += Vector3.right * movementSpeed * Time.deltaTime;
            animator.SetBool("IsMovingBackward", false);
            animator.SetBool("IsMovingForward", true);
        }
    }

    public void DashLeft()
    {
        if (CanGoLeft() && staminaManager.currentStamina >= Constants.DASH_STAMINA_COST && dashEnabled)
        {
            playerRb.AddForce(Vector3.left * dashSpeed, ForceMode.Impulse);
            staminaManager.ApplyDashStaminaCost();
            dashTrailController.EmitDashTrail();
        }
    }

    public void DashRight()
    {
        if (CanGoRight() && staminaManager.currentStamina >= Constants.DASH_STAMINA_COST && dashEnabled)
        {
            playerRb.AddForce(Vector3.right * dashSpeed, ForceMode.Impulse);
            staminaManager.ApplyDashStaminaCost();
            dashTrailController.EmitDashTrail();
        }
    }

    public void Jump()
    {
        if (IsGrounded() && !serveController.isServing)
        {
            playerRb.velocity = new Vector3(playerRb.velocity.x, jumpHeight, 0f);
            OnJump?.Invoke();
        }
    }

    public void FastFall()
    {
        if (canFastFall && fastFallEnabled)
        {
            playerRb.AddForce(new Vector3(0, Constants.FAST_FALL_SPEED * playerRb.mass, 0));
            isFastFalling = true;
        }
        else
        {
            failedFastFalling = true;
        }
    }

    public void CancelFastFall()
    {
        failedFastFalling = false;
    }

    public void SwingRacket(SwingType swingType)
    {
        if (canSwing)
        {
            canSwing = false;
            swingRacket.Swing(swingType);
            OnSwing?.Invoke();
        }
    }

    public void ChangeServeAngle()
    {
        if (serveController.isServing)
        {
            serveController.ChangeServeAngle();
        }
    }

    private bool CanGoLeft()
    {
        // Can't move if serving
        if (serveController.isServing && !moveWhenServingEnabled)
        {
            return false;
        }

        // Check if within court boundaries
        if (playerManager.playerNum == 1)
        {
            return playerTransform.localPosition.x > -Constants.REAR_COURT_X_POS;
        }
        else
        {
            if (serveController.isServing || serveController.isOpponentServing)
            {
                return playerTransform.localPosition.x > Constants.SERVING_LINE_X_POS;
            }
            return playerTransform.localPosition.x > Constants.FRONT_COURT_X_POS;
        }
    }

    private bool CanGoRight()
    {
        // Can't move if serving
        if (serveController.isServing && !moveWhenServingEnabled)
        {
            return false;
        }

        // Check if within court boundaries
        if (playerManager.playerNum == 1)
        {
            if (serveController.isServing || serveController.isOpponentServing)
            {
                return playerTransform.localPosition.x < -Constants.SERVING_LINE_X_POS;
            }
            return playerTransform.localPosition.x < -Constants.FRONT_COURT_X_POS;
        }
        else
        {
            return playerTransform.localPosition.x < Constants.REAR_COURT_X_POS;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            ResetJumpVariables();
        }
    }

    private bool IsGrounded()
    {
        return (playerTransform.localPosition.y <= Constants.GROUND_Y_POS);
    }

    public void SetCanSwing(bool canSwing)
    {
        this.canSwing = canSwing;
    }

    public void ResetJumpVariables()
    {
        canFastFall = false;
        isFastFalling = false;
    }
}
