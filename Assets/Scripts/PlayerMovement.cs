using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    private Transform playerTransform;
    private Rigidbody playerRb;
    public GameObject racket;
    private SwingRacket swingRacket;

    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpHeight = 300f;
    [SerializeField] public bool isPlayerOne;
    [SerializeField] private float gravity = 40;
    private float rearCourtXCoord = 11.4f;
    private float frontCourtXCoord = 0.75f;

    public LayerMask groundLayer;
    public bool isGrounded = true;
    private bool canSwing = true;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerRb = GetComponent<Rigidbody>();
        swingRacket = racket.GetComponent<SwingRacket>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
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

    private void MovePlayer()
    {
        if ((Input.GetKey(KeyCode.A) && isPlayerOne) || (Input.GetKey(KeyCode.LeftArrow) && !isPlayerOne))
        {
            if (CanGoLeft())
            {
                playerTransform.transform.position += Vector3.left * movementSpeed * Time.deltaTime;

            }
        }
        if ((Input.GetKey(KeyCode.D) && isPlayerOne) || (Input.GetKey(KeyCode.RightArrow) && !isPlayerOne))
        {
            if (CanGoRight())
            {
                playerTransform.transform.position += Vector3.right * movementSpeed * Time.deltaTime;
            }
        }
        if (((Input.GetKey(KeyCode.W) && isPlayerOne) || (Input.GetKey(KeyCode.UpArrow) && !isPlayerOne)) && isGrounded)
        {
            playerRb.AddForce(Vector3.up * jumpHeight);
            isGrounded = false;
        }
        if (((Input.GetKey(KeyCode.S) && isPlayerOne) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne)) && canSwing)
        {
            canSwing = false;
            swingRacket.Swing();
        }
    }

    private bool CanGoLeft()
    {
        return (isPlayerOne && playerTransform.position.x > -rearCourtXCoord) ||
            (!isPlayerOne && playerTransform.position.x > frontCourtXCoord);
    }

    private bool CanGoRight()
    {
        return (isPlayerOne && playerTransform.position.x < -frontCourtXCoord) ||
            (!isPlayerOne && playerTransform.position.x < rearCourtXCoord);
    }

    public void SetCanSwing(bool canSwing)
    {
        this.canSwing = canSwing;
    }


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isGrounded = true;
        }
    }
}
