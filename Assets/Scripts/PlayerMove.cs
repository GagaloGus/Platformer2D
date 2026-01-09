using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveStates
{
    Idle, Walk, Run, JumpUp, JumpDown, Slide, Attack
}

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float runSpeedIncrease = 2f;
    public float acceleration = 60f;
    public float airAcceleration = 30f;
    public float friction = 40f;

    [Header("Run Smooth")]
    public float runTransitionSpeed = 4f;
    float currentSpeedMultiplier = 1f;

    [Header("Keys")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float gravityScale = 3f;

    [Header("Raycast")]
    public float raycastStartHeight;
    public float raycastDetectionHeight;
    Vector3 raycastPosition => transform.position + Vector3.down * raycastStartHeight;
    [SerializeField] bool isGrounded;

    Rigidbody2D rb;
    float inputX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.gravityScale = gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        float targetMultiplier = 1f;

        if(Input.GetKey(runKey))
            targetMultiplier *= runSpeedIncrease;

        // Reduce la velocidad poco a poco
        currentSpeedMultiplier = Mathf.MoveTowards(
            currentSpeedMultiplier,
            targetMultiplier,
            runTransitionSpeed * Time.deltaTime
        );

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    void Move()
    {
        float accel = isGrounded ? acceleration : airAcceleration;

        rb.AddForce(Vector2.right * inputX * accel);

        float currentMaxSpeed = maxSpeed * currentSpeedMultiplier;

        // Limitar velocidad maxima (SUAVE)
        if (Mathf.Abs(rb.velocity.x) > currentMaxSpeed)
        {
            rb.velocity = new Vector2(
                Mathf.Sign(rb.velocity.x) * currentMaxSpeed,
                rb.velocity.y
            );
        }

        // Friccion solo cuando deja de moverse
        if (isGrounded && Mathf.Abs(inputX) < 0.01f)
        {
            rb.velocity = new Vector2(
                Mathf.MoveTowards(rb.velocity.x, 0, friction * Time.fixedDeltaTime),
                rb.velocity.y
            );
        }
    }


    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    void CheckGround()
    {
        RaycastHit2D hit = Physics2D.Raycast(raycastPosition, Vector2.down, raycastDetectionHeight, LayerMask.GetMask("Ground"));
        isGrounded = hit.collider != null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if(rb == null)
            rb = GetComponent<Rigidbody2D>();

        float input = rb.velocity.x /8;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(raycastPosition, Vector2.down * raycastDetectionHeight);
        Gizmos.DrawRay(raycastPosition + Vector3.left * 0.25f, Vector2.right * 0.5f);

        Vector3 velPos = transform.position + Vector3.right * input;
        bool right = velPos.x > transform.position.x;
        Gizmos.color = right ? Color.red : Color.cyan;
        Gizmos.DrawRay(transform.position, Vector3.right * input);

        Gizmos.DrawRay(velPos, new Vector2(right ? -1 : 1, 1) / 4);
        Gizmos.DrawRay(velPos, new Vector2(right ? -1 : 1, -1) / 4);

    }
#endif
}
