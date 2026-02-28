using UnityEngine;

public class PlayerKeyboard2D : MonoBehaviour
{
    public enum ControlType { Arrows, WASD }
    public ControlType controls = ControlType.Arrows;

    public Animator animator;
    private SpriteRenderer sr;

    public float moveSpeed = 8f;
    public float jumpForce = 14f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.22f;

    public bool enablePlayerPush = true;
    public float pushStrength = 2.0f;
    public float pushUp = 0.3f;

    private Rigidbody2D rb;

    private bool jumpedThisFrame;
    private bool isJumping;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponent<Animator>();

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        jumpedThisFrame = false;

        // ---------- INPUT ----------
        float x = 0f;
        bool jumpPressed = false;

        if (controls == ControlType.Arrows)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) x = 1f;
            jumpPressed = Input.GetKeyDown(KeyCode.UpArrow);
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) x = -1f;
            if (Input.GetKey(KeyCode.D)) x = 1f;
            jumpPressed = Input.GetKeyDown(KeyCode.W);
        }

        // Grounded una sola vez
        bool grounded = IsGrounded();

        // ---------- JUMP ----------
        if (jumpPressed && grounded)
        {
            Jump();
            jumpedThisFrame = true;
            isJumping = true;

            // Para que no se “bloquee” si spameas salto
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }

        if (grounded && !jumpedThisFrame)
            isJumping = false;

        // ---------- MOVEMENT ----------
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        // ---------- ANIMATOR PARAMS ----------
        bool isWalking = Mathf.Abs(x) > 0.01f;

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsGrounded", grounded);

        animator.SetFloat("XSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("YVelocity", rb.linearVelocity.y);

        animator.SetBool("IsJumping", isJumping);

        // ---------- FLIP ----------
        if (sr != null && Mathf.Abs(x) > 0.01f)
            sr.flipX = (x < 0f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enablePlayerPush) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb == null) return;

        Vector2 dir = (rb.position - otherRb.position);
        if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;
        dir.Normalize();

        Vector2 impulse = new Vector2(dir.x, pushUp).normalized * pushStrength;

        rb.AddForce(impulse, ForceMode2D.Impulse);
        otherRb.AddForce(-impulse, ForceMode2D.Impulse);
    }
}