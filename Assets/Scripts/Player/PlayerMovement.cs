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

    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float jumpHeight = 800f;
    [SerializeField] private float dashSpeed = 35f;

    [SerializeField] private float gravity = 40;

    private bool isGrounded = true;
    private bool canSwing = true;


    // Start is called before the first frame update
    void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        playerTransform = gameObject.transform;
        playerRb = GetComponent<Rigidbody>();
        swingRacket = racket.GetComponent<SwingRacket>();
        staminaManager = GetComponent<StaminaManager>();

        if (GetComponent<ServeController>() != null)
        {
            serveController = GetComponent<ServeController>();
        }
        else
        {
            serveController = GetComponent<OfflineServeController>();
        }
    }

    private void FixedUpdate()
    {
        ApplyGravity();
        ClampPlayerPosition();
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            playerRb.AddForce(new Vector3(0, -gravity * playerRb.mass, 0));
        }
    }

    private void ClampPlayerPosition()
    {
        Vector3 position = transform.position;
        if (playerManager.playerNum == 1)
        {
            position.x = Mathf.Clamp(position.x, -Constants.REAR_COURT_X_POS, -Constants.FRONT_COURT_X_POS);
        }
        else
        {
            position.x = Mathf.Clamp(position.x, Constants.FRONT_COURT_X_POS, Constants.REAR_COURT_X_POS);
        }
        playerTransform.localPosition = position;
    }

    public void MoveLeft()
    {
        if (CanGoLeft())
        {
            playerTransform.localPosition += Vector3.left * movementSpeed * Time.deltaTime;

        }
    }

    public void MoveRight()
    {
        if (CanGoRight())
        {
            playerTransform.localPosition += Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    public void DashLeft()
    {
        if (staminaManager.currentStamina >= Constants.DASH_STAMINA_COST)
        {
            playerRb.AddForce(Vector3.left * dashSpeed, ForceMode.Impulse);
            staminaManager.ApplyDashStaminaCost();
        }
    }

    public void DashRight()
    {
        if (staminaManager.currentStamina >= Constants.DASH_STAMINA_COST)
        {
            playerRb.AddForce(Vector3.right * dashSpeed, ForceMode.Impulse);
            staminaManager.ApplyDashStaminaCost();
        }
    }

    public void Jump()
    {
        if (isGrounded && !serveController.isServing)
        {
            playerRb.velocity = (Vector3.up * jumpHeight);
            isGrounded = false;
        }
    }

    public void SwingRacket(SwingType swingType)
    {
        if (canSwing)
        {
            canSwing = false;
            swingRacket.Swing(swingType);
        }
    }

    public void SetServeAngle(ServeAngle serveAngle)
    {
        if (serveController.isServing)
        {
            serveController.SetServeAngle(serveAngle);
        }
    }

    private bool CanGoLeft()
    {
        // Can't move if serving
        if (serveController.isServing)
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
            return playerTransform.localPosition.x > Constants.FRONT_COURT_X_POS;
        }
    }

    private bool CanGoRight()
    {
        // Can't move if serving
        if (serveController.isServing)
        {
            return false;
        }

        // Check if within court boundaries
        if (playerManager.playerNum == 1)
        {
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
            isGrounded = true;
        }
    }

    public void SetCanSwing(bool canSwing)
    {
        this.canSwing = canSwing;
    }

}
