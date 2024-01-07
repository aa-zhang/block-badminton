using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    private Transform playerTransform;
    private Rigidbody playerRb;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float jumpHeight = 300f;
    [SerializeField] public bool isPlayerOne;
    [SerializeField] private float gravity = 40;
    private float rearCourtXCoord = 11.4f;
    private float frontCourtXCoord = 0.75f;

    public LayerMask groundLayer;
    public bool isGrounded = true;


    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GetComponent<Transform>();
        playerRb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
        ApplyGravity();

    }

    private void FixedUpdate()
    {
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
        if ((Input.GetKey(KeyCode.S) && isPlayerOne) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne))
        {

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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Floor"))
        {
            isGrounded = true;
        }
    }
}
