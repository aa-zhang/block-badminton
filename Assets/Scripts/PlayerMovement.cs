using UnityEngine;
using UnityEngine.Events;

public class PlayerMovement : MonoBehaviour
{

    private Transform playerTransform;
    private Rigidbody playerRb;
    public GameObject racket;
    private SwingRacket swingRacket;

    [SerializeField] private float movementSpeed = 9f;
    [SerializeField] private float jumpHeight = 800f;
    [SerializeField] public bool isPlayerOne;
    [SerializeField] private float gravity = 40;
    [SerializeField] public bool isServing = false;
    private float rearCourtXCoord = 11.4f;
    private float frontCourtXCoord = 0.8f;
    private float servingLineXCoord = 5f;
    public float startingXCoord = 6.14f;

    public LayerMask groundLayer;
    private bool isGrounded = true;
    private bool canSwing = true;
    private bool isControlEnabled = true;


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
            playerTransform.transform.position += Vector3.left * movementSpeed * Time.deltaTime;

        }
    }

    public void MoveRight()
    {
        if (CanGoRight())
        {
            playerTransform.transform.position += Vector3.right * movementSpeed * Time.deltaTime;
        }
    }

    public void Jump()
    {
        if (isGrounded)
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

    //private void MovePlayer()
    //{
    //    if (isControlEnabled)
    //    {
    //        // Move left
    //        if ((Input.GetKey(KeyCode.A) && isPlayerOne) || (Input.GetKey(KeyCode.LeftArrow) && !isPlayerOne))
    //        {
    //            if (CanGoLeft())
    //            {
    //                playerTransform.transform.position += Vector3.left * movementSpeed * Time.deltaTime;

    //            }
    //        }

    //        // Move right
    //        if ((Input.GetKey(KeyCode.D) && isPlayerOne) || (Input.GetKey(KeyCode.RightArrow) && !isPlayerOne))
    //        {
    //            if (CanGoRight())
    //            {
    //                playerTransform.transform.position += Vector3.right * movementSpeed * Time.deltaTime;
    //            }
    //        }

    //        // Jump
    //        if (((Input.GetKey(KeyCode.W) && isPlayerOne) || (Input.GetKey(KeyCode.UpArrow) && !isPlayerOne)) && isGrounded)
    //        {
    //            playerRb.AddForce(Vector3.up * jumpHeight);
    //            isGrounded = false;
    //        }

    //        // Swing racket
    //        if (((Input.GetKey(KeyCode.S) && isPlayerOne) || (Input.GetKey(KeyCode.DownArrow) && !isPlayerOne)) && canSwing)
    //        {
    //            canSwing = false;
    //            swingRacket.Swing();
    //        }
    //    }
    //}

    private bool CanGoLeft()
    {
        if (isPlayerOne)
        {
            return playerTransform.position.x > -rearCourtXCoord;
        }
        else
        {
            if (isServing)
            {
                return playerTransform.position.x > servingLineXCoord;
            }
            else
            {
                return playerTransform.position.x > frontCourtXCoord;
            }
        }

    }

    private bool CanGoRight()
    {

        if (isPlayerOne)
        {
            if (isServing)
            {
                return playerTransform.position.x < -servingLineXCoord;
            }
            else
            {
                return playerTransform.position.x < -frontCourtXCoord;
            }
        }
        else
        {
            return playerTransform.position.x < rearCourtXCoord;
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

    public void SetIsServing(bool isServing)
    {
        this.isServing = isServing;
        if (isServing)
        {
            // Force the serving player to serve behind the serving line
            if (isPlayerOne)
            {
                float boundedX = Mathf.Min(playerTransform.position.x, -servingLineXCoord);
                playerTransform.position = new Vector3(boundedX, playerTransform.position.y, playerTransform.position.z);
            }
            else
            {
                float boundedX = Mathf.Max(playerTransform.position.x, servingLineXCoord);
                playerTransform.position = new Vector3(boundedX, playerTransform.position.y, playerTransform.position.z);
            }
        }
        
    }

    public void SetIsControlEnabled(bool enabled)
    {
        isControlEnabled = enabled;
    }
}
