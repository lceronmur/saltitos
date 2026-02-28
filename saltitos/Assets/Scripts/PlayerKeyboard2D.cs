using UnityEngine;

public class PlayerKeyboard2D : MonoBehaviour
{
    public enum ControlType { Arrows, WASD }
    public ControlType controls = ControlType.Arrows;

    public float moveSpeed = 8f;
    public float jumpForce = 14f;

    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.15f;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Mejor que freezeRotation = true
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void Update()
    {
        float x = 0f;

        if (controls == ControlType.Arrows)
        {
            if (Input.GetKey(KeyCode.LeftArrow)) x = -1f;
            if (Input.GetKey(KeyCode.RightArrow)) x = 1f;

            if (Input.GetKeyDown(KeyCode.UpArrow) && IsGrounded())
                Jump();
        }
        else
        {
            if (Input.GetKey(KeyCode.A)) x = -1f;
            if (Input.GetKey(KeyCode.D)) x = 1f;

            if (Input.GetKeyDown(KeyCode.W) && IsGrounded())
                Jump();
        }

        // Movimiento horizontal controlado (sin vibración)
        rb.linearVelocity = new Vector2(x * moveSpeed, rb.linearVelocity.y);
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;

        return Physics2D.OverlapCircle(
            groundCheck.position,
            groundCheckRadius,
            groundLayer
        );
    }
}