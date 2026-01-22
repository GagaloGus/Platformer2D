using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveStates
{
    Idle, Walk, Run, JumpUp, JumpDown, Slide, AttackMelee, AttackRanged, Crouch
}

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float runSpeedIncrease = 2f;
    public float crouchSpeedIncrease = 0.3f;
    public float acceleration = 40f;
    public float friction = 40f;
    public float slideVelRequired = 10;
    public Vector2 localVel;
    float inputX;



    [Header("Jump")]
    public float jumpForce = 12f;
    public float gravityScale = 3f;

    [Header("Attack")]
    public PlayerBullet Snowball_bullet;
    public float attackDuration;

    [Header("Run Smooth")]
    public float runTransitionSpeed = 5f;
    float currentSpeedMult = 1f, lastTargetSpeedMult;

    [Header("States")]
    public PlayerMoveStates playerMoveState;
    [SerializeField] bool isGrounded, isAttacking, isRunning, isSliding, isCrouching;

    [Header("Keys")]
    public KeyCode runKey = KeyCode.LeftShift;
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode slideKey = KeyCode.S;
    public KeyCode crouchKey = KeyCode.S;
    public KeyCode attack_MeleeKey = KeyCode.Mouse0;
    public KeyCode attack_RangeKey = KeyCode.Mouse1;


    [Header("Raycast")]
    public float raycastStartHeight;
    public float ray_groundedDistance;
    public float ray_groundAngleDistance;
    Vector3 raycastPosition => transform.position + transform.up * -1 * raycastStartHeight;

    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    Transform SpawnBulletPosition, CenterPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();

        SpawnBulletPosition = transform.Find("spawnBall");
        CenterPos = transform.Find("center");
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

        //Estados de animador y variables
        StateMachine();

        //Orientacion del collider segun lo que este haciendo el player
        if (isSliding || isCrouching)
            capsuleCollider.direction = CapsuleDirection2D.Horizontal;
        else
            capsuleCollider.direction = CapsuleDirection2D.Vertical;

        //Multiplicadores de velocidad
        if (isRunning || isSliding)
            targetSpeedMult *= runSpeedIncrease;

        if (isCrouching)
            targetSpeedMult = crouchSpeedIncrease;

        if (isGrounded)
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

    void StateMachine()
    {
        if (!isGrounded) //En el aire
        {
            playerMoveState = rb.velocity.y < 0 ? PlayerMoveStates.JumpDown : PlayerMoveStates.JumpUp;
            isRunning = false;
            isSliding = false;
        }
        else
        {
            if (Input.GetKeyDown(attack_MeleeKey) && !isAttacking && !isSliding) //Ataque melee
            {
                StartCoroutine(AttackCoroutine(PlayerMoveStates.AttackMelee));
            }
            else if (Mathf.Abs(inputX) > 0.01f) //Movimiento lateral
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
            else //Quieto
            {
                playerMoveState = PlayerMoveStates.Idle;
                isRunning = false;
                isSliding = false;
            }

            if (!isSliding && !isRunning && isGrounded && Input.GetKey(crouchKey))
            {
                isCrouching = true;
                playerMoveState = PlayerMoveStates.Crouch;
            }
            else
                isCrouching = false;

        }

        if (Input.GetKeyDown(attack_RangeKey) && !isAttacking && !isSliding) //Ataque rango que tambien se puede hacer en el aire
        {
            StartCoroutine(AttackCoroutine(PlayerMoveStates.AttackRanged));
        }
    }

    void FixedUpdate()
    {
        CheckGround();
        Move();

        //Aplica una fuerza para que el jugador se "pegue" al suelo
        if (Mathf.Abs(localVel.x) > 2f && isGrounded && (isRunning || isSliding))
        {
            rb.gravityScale = 0;
            rb.AddForce(transform.up * -1 * 5);
        }
        else
            rb.gravityScale = gravityScale;
    }

    void Move()
    {
        float currentMaxSpeed = maxSpeed * currentSpeedMult;

        //Velocidad la transforma en dimensiones locales
        localVel = transform.InverseTransformDirection(rb.velocity);

        //Evita que el jugador se mueva al atacar
        if (!isAttacking)
            rb.AddForce(transform.right * inputX * acceleration);


        //Limita la velocidad en el eje local
        localVel.x = Mathf.Clamp(localVel.x, -currentMaxSpeed, currentMaxSpeed);

        //rb.velocity = new Vector2(currentMaxSpeed * (rb.velocity.x > 0 ? 1 : -1), rb.velocity.y);
        //Vuelve a transformar a dimensiones globales
        rb.velocity = transform.TransformDirection(localVel);

        // Friccion solo cuando deja de moverse
        if (isGrounded && Mathf.Abs(inputX) < 0.01f)
        {
            rb.velocity = new Vector2(
                Mathf.MoveTowards(rb.velocity.x, 0, friction * Time.fixedDeltaTime),
                rb.velocity.y
            );
        }
    }

    IEnumerator AttackCoroutine(PlayerMoveStates attackType)
    {
        isAttacking = true;
        playerMoveState = attackType;
        if (attackType == PlayerMoveStates.AttackRanged)
        {
            PlayerBullet bullet = Instantiate(Snowball_bullet);
            bullet.moveRight = transform.localScale.x > 0;
            bullet.transform.position = SpawnBulletPosition.position;
        }

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    void CheckGround()
    {
        RaycastHit2D groundedCast = Physics2D.Raycast(raycastPosition, transform.up * -1, ray_groundedDistance, LayerMask.GetMask("Ground"));
        isGrounded = groundedCast.collider != null;

        RaycastHit2D groundCheckCast = Physics2D.Raycast(raycastPosition, transform.up * -1, ray_groundAngleDistance*2, LayerMask.GetMask("Ground"));
        float slopeAngle = Vector2.SignedAngle(groundCheckCast.normal, Vector2.up)*-1;
        transform.rotation = Quaternion.Euler(0f, 0f, slopeAngle);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (CenterPos == null)
            CenterPos = transform.Find("center");

        float input = localVel.x / 8;

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(raycastPosition + transform.right * -0.25f, transform.right * 0.5f);
        Gizmos.DrawRay(raycastPosition + transform.right*0.1f, transform.up * -1 * ray_groundedDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(raycastPosition + transform.right*-0.1f, transform.up * -1 * ray_groundAngleDistance);
        Gizmos.DrawWireSphere(raycastPosition, 0.1f);

        bool right = input > 0;
        Gizmos.color = right ? Color.red : Color.cyan;
        Gizmos.DrawRay(CenterPos.position, transform.right * input);

        //Gizmos.DrawRay(velPos, new Vector2(right ? -1 : 1, 1) / 4);
        //Gizmos.DrawRay(velPos, new Vector2(right ? -1 : 1, -1) / 4);

    }
#endif
}
