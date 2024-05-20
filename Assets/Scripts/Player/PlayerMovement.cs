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

    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float jumpHeight = 800f;
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
    }

    private void ApplyGravity()
    {
        if (!isGrounded)
        {
            playerRb.AddForce(new Vector3(0, -gravity * playerRb.mass, 0));
        }
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

    public void Jump()
    {
        if (isGrounded && !serveController.isServing)
        {
            playerRb.velocity = (Vector3.up * jumpHeight);
            isGrounded = false;
        }
    }

    public void SwingRacket()
    {
        if (canSwing)
        {
            canSwing = false;
            swingRacket.Swing();
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
