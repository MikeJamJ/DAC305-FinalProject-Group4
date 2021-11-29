using UnityEngine;

public class Character2Dcontroller : MonoBehaviour
{
    [SerializeField] private float speedX;          // defualt speed multiplier
    [SerializeField] private float jumpPower;       // jump velocity 
    [SerializeField] private float sprintPower;     // sprint x velocity multiplier
    [SerializeField] private LayerMask groundLayer; // ground object layer
    //[SerializeField] private LayerMask wallLayer;   // wall object layer 

    private Rigidbody2D body;           // physics body of character
    private Animator animator;          // animator of character
    private BoxCollider2D boxCollider;  // box collider of character (for climbing)
    private CapsuleCollider2D capsuleCollider; // capsule collider for character (for hit registration)

    private float walljumpcooldown = 0.0f;
    private bool sprint = false;

    private float maxVelocityX = 8.0f;

    // States that save the characters last action
    private bool groundedState;
    private bool jumpingState;
    private bool climbingState;

    // Initialize character objects on startup
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Get movement direction from key inputs
        float movementX = Input.GetAxis("Horizontal");
        float movementY = Input.GetAxis("Vertical");

        //Flip the Character based on X movement
        flipCharacter(body.velocity.x);

        // Set Animator Parameters
        animator.SetBool("Run", movementX != 0);
        animator.SetBool("GroundCheck", isGrounded());
        animator.SetBool("WallCheck", onWall());
        animator.SetBool("Sprint", sprint);
        animator.SetBool("Fall", (body.velocity.y < -0.01f) && !isGrounded());

        // Handle all logic for when character is on the ground
        if (isGrounded())
        {
            body.gravityScale = 1;  // enable gravity

            jumpingState = false;   // set jumping boolean to false

            // Set velocity of character
            body.velocity = new Vector2(movementX * speedX, body.velocity.y);

            // Check that sprint key is pressed and player is moving
            if (Input.GetKey(KeyCode.LeftShift) && movementX != 0)
            {
                // Check that the player is not on the wall
                if (!onWall())
                {
                    // Multiply velocity by sprintPower
                    body.velocity = new Vector2(body.velocity.x * sprintPower, body.velocity.y);
                    // set max velocity
                    maxVelocityX = body.velocity.x;
                    // Set sprint to true to start sprint animation
                    sprint = true;
                }
            }
            else
            {
                // set max velocity
                maxVelocityX = speedX;
                // Set sprint to false to turn off sprint animation
                sprint = false;

            }

            // Check if spacebar has been pressed to enact jumping mechanic
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        // Handle all logic for when character is on a wall
        else if (onWall())
        {
            body.gravityScale = 0;  // disable gravity

            if (jumpingState)
            {
                body.velocity = new Vector2(body.velocity.x, 0);
                jumpingState = false;
            }

            // Set velocity of character
            body.velocity = new Vector2(body.velocity.x, movementY * speedX);

            // Check if spacebar has been pressed to enact jumping mechanic
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }

        }
        // Handle all logic for when the character is in the air
        else
        {
            body.gravityScale = 1;  // enable gravity

            if (Mathf.Abs(body.velocity.x + movementX) <= maxVelocityX)
            {
                body.velocity = new Vector2(body.velocity.x + movementX, body.velocity.y);
            }
            else
            {
                body.velocity = new Vector2(body.velocity.x, body.velocity.y);
            }
        }
    }

    // Function for handling jumping logic
    private void Jump()
    {
        if (!isGrounded())
        {
            // Increase y velocity of character if they are on the ground
            body.velocity = new Vector2(body.velocity.x, jumpPower);
        }
        else if (onWall())
        {
            walljumpcooldown = 0;
            /*
            Get characters transform: transform.localScale.x
            Check whether character is facing right or left: Mathf.Sign()
            Make the value negative to move the character away from the wall: -
            Multiply by speedX to increase x velocity
            */
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * speedX, jumpPower);
        }
        // Set animator trigger to start animation
        animator.SetTrigger("Jump");
        jumpingState = true;
    }

    // Function for checking whether the character is on the ground
    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(capsuleCollider.bounds.center, capsuleCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        return raycastHit.collider != null;
    }

    // Function for checking whether the character is on a wall
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.05f, groundLayer);
        return raycastHit.collider != null;
    }

    // Function for fliping the character dependiing on the horizontal movement
    void flipCharacter(float movement)
    {
        if (movement > 0.01f)
        {
            Vector3 tempScale = transform.localScale;
            tempScale.x = 1;
            transform.localScale = tempScale;
        }
        else if (movement < -0.01f)
        {
            Vector3 tempScale = transform.localScale;
            tempScale.x = -1;
            transform.localScale = tempScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
    }
}
