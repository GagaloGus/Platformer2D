using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveStates
{
    Idle, Walk, Run, JumpUp, JumpDown, Slide, Attack
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float runSpeedIncrease = 2f;
    public float acceleration = 40f;
    public float friction = 40f;
    public float slideVelRequired = 10;

    [Header("Run Smooth")]
    public float runTransitionSpeed = 5f;
    float currentSpeedMult = 1f, lastTargetSpeedMult;

    [Header("States")]
    public PlayerMoveStates playerMoveState;
    public SpriteRenderer spriteRend;
    [SerializeField] bool isGrounded, isAttacking, isRunning, isSliding;
    public float attackDuration;

    [Header("Keys")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.S;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float gravityScale = 3f;

    [Header("Raycast")]
    public float raycastStartHeight;
    public float raycastDetectionHeight;
    Vector3 raycastPosition => transform.position + Vector3.down * raycastStartHeight;

    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    float inputX;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRend = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    void Start()
    {
        rb.gravityScale = gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");

        float targetSpeedMult = 1f;

        //Animator + velocidad aumentada
        if (!isGrounded)
        {
            playerMoveState = rb.velocity.y < 0 ? PlayerMoveStates.JumpDown : PlayerMoveStates.JumpUp;
            isAttacking = false;
        }
        else
        {
            if (Input.GetKeyDown(attackKey) && !isAttacking && !isSliding)
            {
                StartCoroutine(AttackCoroutine());
            }
            else if (Mathf.Abs(inputX) > 0.01f)
            {
                if (Input.GetKey(runKey))
                {
                    if (Input.GetKey(slideKey) && Mathf.Abs(rb.velocity.x) >= slideVelRequired)
                    {
                        isRunning = false;
                        isSliding = true;
                        playerMoveState = PlayerMoveStates.Slide;
                    }
                    else
                    {
                        isRunning = true;
                        isSliding = false;
                        playerMoveState = PlayerMoveStates.Run;
                    }
                }
                else
                {
                    isRunning = false;
                    isSliding = false;
                    playerMoveState = PlayerMoveStates.Walk;
                }

                // Flipear el sprite en vez de la escala
                if (inputX > 0.01f && transform.localScale.x < 0)
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                if (inputX < -0.01f && transform.localScale.x > 0)
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
            }
            else
                playerMoveState = PlayerMoveStates.Idle;
        }

        capsuleCollider.direction = isSliding ? CapsuleDirection2D.Horizontal : CapsuleDirection2D.Vertical;

        if(isRunning || isSliding)
            targetSpeedMult *= runSpeedIncrease;
        
        if(isGrounded)
            lastTargetSpeedMult = targetSpeedMult;
        else
            targetSpeedMult = lastTargetSpeedMult;


            // Reduce la velocidad poco a poco
            currentSpeedMult = Mathf.MoveTowards(
                currentSpeedMult,
                targetSpeedMult,
                runTransitionSpeed * Time.deltaTime
            );

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();

        animator.SetInteger("player_states", (int)playerMoveState);
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();
    }

    void Move()
    {
        //Evita que el jugador se mueva al atacar
        if (!isAttacking)
            rb.AddForce(Vector2.right * inputX * acceleration);

        float currentMaxSpeed = maxSpeed * currentSpeedMult;

        // Limitar velocidad maxima (SUAVE)
        if (Mathf.Abs(rb.velocity.x) > currentMaxSpeed)
        {
            rb.velocity = new Vector2(
               currentMaxSpeed * (rb.velocity.x > 0 ? 1 : -1),
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

    IEnumerator AttackCoroutine()
    {
        isAttacking = true;
        playerMoveState = PlayerMoveStates.Attack;
        yield return new WaitForSeconds(attackDuration);
        isAttacking=false;
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
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        float input = rb.velocity.x / 8;

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
