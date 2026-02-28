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

    [Header("Jump stability")]
    public float coyoteTime = 0.08f;     // tolerancia si pierde suelo 1 frame
    public float jumpBufferTime = 0.10f; // tolerancia si presiona salto un poco antes

    [Header("Anti-sticky + Repel BOTH players")]
    public bool antiStick = true;
    public float separationDistance = 0.08f;
    public float bumpImpulse = 1.8f;
    public float separationCooldown = 0.05f;
    public float blockSpeedDamping = 0f;

    private Rigidbody2D rb;

    private float inputX;
    private float lastSeparateTime;

    // timers para salto estable
    private float coyoteTimer;
    private float jumpBufferTimer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (animator == null) animator = GetComponentInChildren<Animator>();

        sr = GetComponent<SpriteRenderer>();
        if (sr == null) sr = GetComponentInChildren<SpriteRenderer>();

        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        // -------- INPUT --------
        inputX = 0f;

        if (controls == ControlType.Arrows)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) inputX = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) inputX = 1f;

            if (Input.GetKeyDown(KeyCode.UpArrow))
                jumpBufferTimer = jumpBufferTime;
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) inputX = -1f;
            if (Input.GetKey(KeyCode.D)) inputX = 1f;

            if (Input.GetKeyDown(KeyCode.W))
                jumpBufferTimer = jumpBufferTime;
        }

        // -------- Timers --------
        if (jumpBufferTimer > 0f) jumpBufferTimer -= Time.deltaTime;

        bool groundedNow = IsGroundedRaw();
        if (groundedNow) coyoteTimer = coyoteTime;
        else coyoteTimer -= Time.deltaTime;

        // -------- Animator --------
        animator.SetBool("IsWalking", Mathf.Abs(inputX) > 0.01f);
        animator.SetBool("IsGrounded", groundedNow);
        animator.SetFloat("XSpeed", Mathf.Abs(rb.linearVelocity.x));
        animator.SetFloat("YVelocity", rb.linearVelocity.y);

        if (sr != null && Mathf.Abs(inputX) > 0.01f)
            sr.flipX = (inputX < 0f);
    }

    void FixedUpdate()
    {
        // Movimiento horizontal
        rb.linearVelocity = new Vector2(inputX * moveSpeed, rb.linearVelocity.y);

        // Salto estable: si hay buffer y coyote, salta
        if (jumpBufferTimer > 0f && coyoteTimer > 0f)
        {
            DoJump();
            jumpBufferTimer = 0f;
            coyoteTimer = 0f;

            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }
    }

    void DoJump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    // --- Ground check estable ---
    bool IsGroundedRaw()
    {
        if (groundCheck == null) return false;

        // 1) suelo real por layer
        bool onGround = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        if (onGround) return true;

        // 2) permitir “pararse” encima del otro player
        Collider2D[] hits = Physics2D.OverlapCircleAll(groundCheck.position, groundCheckRadius);
        foreach (var h in hits)
        {
            if (h == null) continue;
            if (h.transform == transform || h.transform.IsChildOf(transform)) continue;

            if (h.CompareTag("Player"))
            {
                // solo cuenta si voy cayendo o casi quieto (evita que cuente subiendo)
                if (rb.linearVelocity.y <= 0.1f)
                    return true;
            }
        }

        return false;
    }

    // --- Repel to BOTH players ---
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!antiStick) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        Rigidbody2D otherRb = collision.rigidbody;
        if (otherRb == null) return;

        if (Time.time - lastSeparateTime < separationCooldown) return;

        Vector2 n = collision.GetContact(0).normal;

        // solo choques laterales
        if (Mathf.Abs(n.x) <= Mathf.Abs(n.y)) return;

        float otherDir = Mathf.Sign(collision.transform.position.x - transform.position.x);

        // si presiono hacia el otro, bloqueo mi empuje
        if (Mathf.Abs(inputX) > 0.01f && Mathf.Sign(inputX) == otherDir)
            rb.linearVelocity = new Vector2(inputX * moveSpeed * blockSpeedDamping, rb.linearVelocity.y);

        // separación simétrica
        rb.MovePosition(rb.position + new Vector2(-otherDir * separationDistance, 0f));
        otherRb.MovePosition(otherRb.position + new Vector2(+otherDir * separationDistance, 0f));

        // impulso para ambos
        Vector2 impulse = new Vector2(-otherDir, 0f) * bumpImpulse;
        rb.AddForce(impulse, ForceMode2D.Impulse);
        otherRb.AddForce(-impulse, ForceMode2D.Impulse);

        lastSeparateTime = Time.time;
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}