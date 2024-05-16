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
            playerTransform.position += Vector3.left * movementSpeed * Time.deltaTime;

        }
    }

    public void MoveRight()
    {
        if (CanGoRight())
        {
            playerTransform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    public void Jump()
    {
        if (isGrounded && !serveController.isServing)
        {
            playerRb.AddForce(Vector3.up * jumpHeight);
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

    private bool CanGoLeft()
    {
        if (playerManager.playerNum == 1)
        {
            return playerTransform.position.x > -Constants.REAR_COURT_X_POS;
        }
        else
        {
            if (serveController.isServing)
            {
                return playerTransform.position.x > Constants.SERVE_X_POS;
            }
            else
            {
                return playerTransform.position.x > Constants.FRONT_COURT_X_POS;
            }
        }
    }

    private bool CanGoRight()
    {
        if (playerManager.playerNum == 1)
        {
            if (serveController.isServing)
            {
                return playerTransform.position.x < -Constants.SERVE_X_POS;
            }
            else
            {
                return playerTransform.position.x < -Constants.FRONT_COURT_X_POS;
            }
        }
        else
        {
            return playerTransform.position.x < Constants.REAR_COURT_X_POS;
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
