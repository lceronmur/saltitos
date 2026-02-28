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
    public float groundCheckRadius = 0.25f;

    [Header("Repel")]
    public bool enablePlayerPush = true;
    public float pushStrength = 2.0f;
    public float pushUp = 0.15f;

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

        bool grounded = IsGrounded();

        if (jumpPressed && grounded)
        {
            Jump();
            jumpedThisFrame = true;
            isJumping = true;

            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }

        if (grounded && !jumpedThisFrame)
            isJumping = false;

        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);

        bool isWalking = Mathf.Abs(x) > 0.01f;

        animator.SetBool("IsWalking", isWalking);
        animator.SetBool("IsGrounded", grounded);
        animator.SetFloat("XSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("YVelocity", rb.linearVelocity.y);
        animator.SetBool("IsJumping", isJumping);

        if (sr != null && Mathf.Abs(x) > 0.01f)
            sr.flipX = (x < 0f);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // ✅ Suelo = Ground layer OR encima de otro Player (Tag)
    bool IsGrounded()
    {
        if (groundCheck == null) return false;

        bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // detectar otro player como suelo
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
        bool onOtherPlayer = false;

        foreach (var h in hits)
        {
            if (h == null) continue;

            // ignorar mis colliders
            if (h.transform == transform || h.transform.IsChildOf(transform)) continue;

            if (h.CompareTag("Player"))
            {
                onOtherPlayer = true;
                break;
            }
        }

        // solo cuenta como suelo si estás cayendo o casi quieto (evita pegarse subiendo)
        if (onOtherPlayer && rb.linearVelocity.y > 0.1f) onOtherPlayer = false;

        return onGround || onOtherPlayer;
    }

    // ✅ Repulsión solo lateral (no cuando caen encima)
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!enablePlayerPush) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        var otherRb = collision.rigidbody;
        if (otherRb == null) return;

        Vector2 n = collision.GetContact(0).normal;

        // choque lateral = empujar
        if (Mathf.Abs(n.x) > Mathf.Abs(n.y))
        {
            float dir = Mathf.Sign(n.x);
            Vector2 impulse = new Vector2(dir, pushUp).normalized * pushStrength;

            rb.AddForce(impulse, ForceMode2D.Impulse);
            otherRb.AddForce(-impulse, ForceMode2D.Impulse);
        }
    }

    // (opcional) dibuja el círculo del groundcheck para debug
    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
   
}