using UnityEngine;

public class Character2Dcontroller : MonoBehaviour
{
    [SerializeField] private float speedX;          // defualt speed multiplier
    [SerializeField] private float jumpPower;       // jump velocity 
    [SerializeField] private float sprintPower;     // sprint x velocity multiplier
    [SerializeField] private LayerMask groundLayer; // ground object layer
    [SerializeField] private int sprintStamina;     // stamina used when sprinting
    [SerializeField] private int climbStamina;      // stamina used when climbing
    [SerializeField] private int sprintJumpStamina; // stamina used when jumping after sprinting

    private Rigidbody2D body;           // physics body of character
    private Animator animator;          // animator of character
    private BoxCollider2D boxCollider;  // box collider of character (for climbing)
    private CapsuleCollider2D capsuleCollider; // capsule collider for character (for hit registration)
    private Rigidbody2D snailRb;        // rigibody of snail     

    private float maxVelocityX = 8.0f;              // Max velocity of the character
    private float walljumpcooldown = 0.0f;          // Cooldown of wall jump
    private float jumpMovementSmoothing = 0.07f;    // Multiplier for smoothing movement when character is in the air

    // States that save the characters last action
    private bool jumpingState = false;
    private bool sprintJumpState = false;
    private bool sprintState = false;

    private Average averageDist = new Average();

    // Initialize character objects on startup
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        snailRb = GameObject.FindGameObjectWithTag("Snail").GetComponent<Rigidbody2D>();
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
        animator.SetBool("GroundCheck", isGrounded());
        animator.SetBool("WallCheck", onWall() && StaminaBar.instance.getStamina() > 0);
        animator.SetBool("Run", movementX != 0);
        animator.SetBool("Sprint", sprintState);
        animator.SetBool("Fall", (body.velocity.y < -0.3f) && !animator.GetBool("WallCheck"));

        // Need to check if game is paused to not update Time.timeScale
        if (!GameManager.gameIsPaused) {
            getSnailDistance();
        }
        
        // Handle all logic for when character is on the ground
        if (isGrounded())
        {
            body.gravityScale = 1;  // enable gravity

            // set jumping booleans to false
            jumpingState = false;
            sprintJumpState = false;

            // Set velocity of character
            body.velocity = new Vector2(movementX * speedX, body.velocity.y);

            // Check if there is stamina to be used and if the sprint key is pressed and if player is moving
            if ((StaminaBar.instance.getStamina() > 0) && Input.GetKey(KeyCode.LeftShift) && (movementX != 0))
            {
                // Check that the player is not on the wall
                if (!onWall())
                {
                    // Multiply velocity by sprintPower
                    body.velocity = new Vector2(body.velocity.x * sprintPower, body.velocity.y);
                    // set max velocity to the current speed of player
                    // ensures player cannot go faster than the speed they jump at
                    maxVelocityX = Mathf.Abs(body.velocity.x);
                    // Set sprint to true to start sprint animation
                    sprintState = true;
                    // Use stamina
                    StaminaBar.instance.UseStamina(sprintStamina);
                }
            }
            else
            {
                // set max velocity to default speed
                maxVelocityX = speedX;
                // Set sprint to false to turn off sprint animation
                sprintState = false;
            }

            // Check if spacebar has been pressed to enact jumping mechanic
            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
                // Set if character was sprinting
                if (sprintState) sprintJumpState = true;
                else jumpingState = true;
            }
        }
        // Handle all logic for when character is on a wall
        else if (onWall())
        {
            // Check if there is stamina to be used
            if (StaminaBar.instance.getStamina() > 0)
            {
                // Check if cooldown is over
                // This is here to make sure you don't keep wall jumping
                if (walljumpcooldown > 0.2f)
                {
                    body.gravityScale = 0;  // disable gravity

                    // Check if character got on the wall due to jumping
                    if (jumpingState)
                    {
                        body.velocity = new Vector2(body.velocity.x, 0);
                        jumpingState = false;
                    }

                    // Set velocity of character
                    body.velocity = new Vector2(body.velocity.x, speedX * movementY);

                    // Use stamina if the character is moving up or down on a wall
                    if (movementY > 0.01f || movementY < -0.01f)
                    {
                        StaminaBar.instance.UseStamina(climbStamina);
                    }

                    // Check if spacebar has been pressed to enact jumping mechanic
                    if (Input.GetKey(KeyCode.Space))
                    {
                        Jump();
                        jumpingState = true;
                    }
                }
                else
                {
                    walljumpcooldown += Time.deltaTime;
                }
            }
            else
            {
                body.gravityScale = 1;
            }

        }
        // Handle all logic for when the character is in the air
        else
        {
            body.gravityScale = 1;  // enable gravity

            // Add any user input to the character x velocity
            body.velocity = new Vector2(body.velocity.x + (movementX * jumpMovementSmoothing), body.velocity.y);

            // If character is going faster than they should, set x velocity to max
            if (Mathf.Abs(body.velocity.x) >= Mathf.Abs(maxVelocityX))
            {
                // Need to multiply the max velocity by the direction the character is facing
                body.velocity = new Vector2(Mathf.Sign(transform.localScale.x) * maxVelocityX, body.velocity.y);
            }

            // Use stamina if the character sprinted before jumping
            if (sprintJumpState)
            {
                StaminaBar.instance.UseStamina(sprintJumpStamina);
            }
        }

    }

    // Function for handling jumping logic
    private void Jump()
    {
        if (isGrounded())
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

    // Function for fliping the character depending on the horizontal movement
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

    void OnCollisionStay2D(Collision2D col) {
        if (col.gameObject.tag == "Snail") {
            GameManager.instance.GameOver();
        }
    }

    void getSnailDistance() {
        float minDist = 4f;
        float dist = Vector2.Distance(body.position, snailRb.position);
        if (dist < minDist) {
            Time.timeScale = 0.5f;
        } else {
            Time.timeScale = 1f;
        }
        averageDist.update(dist);
    }

    // Fucntion for returning the average distance
    public float getAverage() {
        return averageDist.getAverage();
    }
}
