using UnityEngine;

public class Character2Dcontroller : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private float jumpPower;
    private Rigidbody2D body;
    private Animator animator;
    private BoxCollider2D boxCollider;
    [SerializeField]private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    private float walljumpcooldown;
    private bool dash = false;
    private void Start()
    {
        body = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    private void Update()
    {
        float movement = Input.GetAxis("Horizontal");
        body.velocity = new Vector2(movement * speed, body.velocity.y);

        //Flip the Character
        if (movement > 0.01f)
            transform.localScale = new Vector3(3,3,3);
        else if (movement < -0.01f)
            transform.localScale = new Vector3(-3, 3, 3);


        // Animator Parameters
        animator.SetBool("Run", movement != 0);
        animator.SetBool("GroundCheck", isGrounded());
        animator.SetBool("WallCheck", onWall());
        animator.SetBool("Sprint", dash);
        if (walljumpcooldown > 0.2f)
        {

            body.velocity = new Vector2(movement * speed, body.velocity.y);

            if (onWall() && !isGrounded())
            {
                body.gravityScale = 0;
                body.velocity = Vector2.zero;
            }
            else
                body.gravityScale = 1;

            if (Input.GetKey(KeyCode.Space))
            {
                Jump();
            }
        }
        else walljumpcooldown += Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift) && movement != 0)
        {
            body.velocity = body.velocity * 2;
            dash = true;
        }
        else
            dash = false;
    }

    private void Jump()
    {
        if (isGrounded())
        {
            body.velocity = new Vector2(body.velocity.x, jumpPower);
            animator.SetTrigger("jump");
        }
        else if (onWall() && !isGrounded())
        {
            walljumpcooldown = 0;
            body.velocity = new Vector2(-Mathf.Sign(transform.localScale.x) * 3, 4);
        }

    }
        

    void OnCollisionEnter2D(Collision2D collision)
    {
    }
    

    private bool isGrounded()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, Vector2.down, 0.05f, groundLayer);
        return raycastHit.collider != null;
    }
    private bool onWall()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxCollider.bounds.size, 0, new Vector2(transform.localScale.x, 0), 0.05f, wallLayer);
        return raycastHit.collider != null;
    }
}
