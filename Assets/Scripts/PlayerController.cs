using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerMoveStates
{
    Idle, Walk, Run, JumpUp, JumpDown, Slide, AttackMelee, AttackRanged, Crouch, Death
}

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

    [Header("States")]
    public int life;
    public PlayerMoveStates playerMoveState;
    public bool canMove, isGrounded, isAttacking, isRunning, isSliding, isCrouching, hasDied;

    [Header("Movement")]
    public float maxSpeed = 3f;
    public float runSpeedIncrease = 2.5f;
    public float slideSpeedIncrease = 3.5f;
    public float crouchSpeed = 0.3f;
    public float acceleration = 40f;
    public float friction = 40f;
    public float slideVelRequired = 10;
    public Vector2 localVel;
    public float YLevelDeath = -7.5f;
    float inputX;
    Vector2 startPos;

    [Header("Jump")]
    public float jumpForce = 12f;
    public float gravityScale = 3f;

    [Header("Attack")]
    public PlayerBullet Snowball_bullet;
    public int dmgMeleeAtk, dmgChargeAtk;
    public float attackDuration, hitInvFramesDuration;
    float iFrameCounter;


    [Header("Run Smooth")]
    public float runTransitionSpeed = 5f;
    float currentSpeedMult = 1f, lastTargetSpeedMult;

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
    Vector2 raycastPosition => transform.position + transform.up * -1 * raycastStartHeight;

    [Header("Callbacks")]
    public Action onStartCrouch, onStopCrouch, onStartSlide;
    public Action<Enemy> onChargeOnEnemy;

    Rigidbody2D rb;
    CapsuleCollider2D capsuleCollider;
    SpriteRenderer sprtRenderer;
    Animator animator;
    Transform SpawnBulletPosition, CenterPos, Casco, Espada;
    cam Cam;


    private void Awake()
    {
        instance = this;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        sprtRenderer = GetComponent<SpriteRenderer>();
        Cam = FindObjectOfType<cam>();

        SpawnBulletPosition = transform.Find("spawnBall");
        CenterPos = transform.Find("center");
        Casco = transform.Find("helmet");
        Espada = transform.Find("sword");

        onStartCrouch = () =>
        {
            AudioManager.instance.PlayLoopedSFX(MusicLibrary.instance.player_crouching_sfx);
        };

        onStopCrouch = () =>
        {
            AudioManager.instance.StopLoopedSFX();
        };

        onStartSlide = () =>
        {
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.player_slide_sfx);
        };

        onChargeOnEnemy = (Enemy enemy) =>
        {
            //No muere por impacto
            if (enemy.health > dmgChargeAtk)
            {
                Vector2 bounceDir = new Vector2(CenterPos.position.x - enemy.transform.position.x, 10).normalized;
                AddForceToDir(bounceDir, 10);
                AudioManager.instance.PlaySFX2D(MusicLibrary.instance.player_bump_sfx);
            }
        };
    }

    void Start()
    {
        canMove = true;
        hasDied = false;
        startPos = transform.position;
        rb.gravityScale = gravityScale;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("player_states", (int)playerMoveState);

        if (hasDied)
            return;

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
        if (isRunning)
            targetSpeedMult *= runSpeedIncrease;
        else if (isSliding)
            targetSpeedMult *= slideSpeedIncrease;

        if (isCrouching)
            targetSpeedMult = crouchSpeed;

        if (isGrounded)
            lastTargetSpeedMult = targetSpeedMult;
        else
            targetSpeedMult = lastTargetSpeedMult;

        if (Input.GetKeyDown(jumpKey) && isGrounded)
            Jump();

        // Reduce la velocidad poco a poco
        currentSpeedMult = Mathf.MoveTowards(
            currentSpeedMult,
            targetSpeedMult,
            runTransitionSpeed * Time.deltaTime
        );

        if (transform.position.y < YLevelDeath)
            transform.position = startPos;


    }

    void FixedUpdate()
    {
        //Rayos disparados hacia el suelo
        RaycastHit2D groundedCast = Physics2D.Raycast(raycastPosition, transform.up * -1, ray_groundedDistance, LayerMask.GetMask("Ground"));
        RaycastHit2D groundCheckCast = Physics2D.Raycast(raycastPosition, transform.up * -1, ray_groundAngleDistance, LayerMask.GetMask("Ground"));

        isGrounded = groundedCast.collider != null;

        if (hasDied)
            return;

        float slopeAngle = Vector2.SignedAngle(groundCheckCast.normal, Vector2.up) * -1;
        transform.rotation = Quaternion.Euler(0f, 0f, slopeAngle);

        Move();

        //Aplica una fuerza para que el jugador se "pegue" al suelo
        if (Mathf.Abs(localVel.x) > 2f && isGrounded && (isRunning || isSliding))
            rb.gravityScale = 0;
        else
            rb.gravityScale = gravityScale;

        rb.AddForce(-groundCheckCast.normal * 20f, ForceMode2D.Force);
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
            else if (Mathf.Abs(localVel.x) > 0.01f) //Movimiento lateral
            {
                if (Input.GetKey(runKey))
                {
                    if (Input.GetKey(slideKey) && Mathf.Abs(localVel.x) >= slideVelRequired)
                    {
                        if (!isSliding)
                            onStartSlide();

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
                if (localVel.x > 0.01f && transform.localScale.x < 0)
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                if (localVel.x < -0.01f && transform.localScale.x > 0)
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
                if (!isCrouching)
                    onStartCrouch();

                isCrouching = true;
                playerMoveState = PlayerMoveStates.Crouch;
            }
            else
            {
                if (isCrouching)
                    onStopCrouch();

                isCrouching = false;
            }

        }

        if (Input.GetKeyDown(attack_RangeKey) && !isAttacking && !isSliding) //Ataque rango que tambien se puede hacer en el aire
        {
            StartCoroutine(AttackCoroutine(PlayerMoveStates.AttackRanged));
        }
    }

    void Jump()
    {
        rb.AddForce(transform.up * jumpForce, ForceMode2D.Impulse);
    }

    void Move()
    {
        if (!canMove)
            return;

        float currentMaxSpeed = maxSpeed * currentSpeedMult;

        //Velocidad la transforma en dimensiones locales
        localVel = transform.InverseTransformDirection(rb.velocity);

        //Evita que el jugador se mueva al atacar
        if (!isAttacking)
            rb.AddForce(transform.right * inputX * acceleration);


        //Limita la velocidad en el eje local
        localVel.x = Mathf.Clamp(localVel.x, -currentMaxSpeed, currentMaxSpeed);

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

    //El jugador ha recibido daño
    public void Hit(int healthReduce, Vector2 bounceDir, GameObject hitObject)
    {
        life -= healthReduce;
        if (life <= 0)
        {
            Death(hitObject);
        }
        else
        {
            AddForceToDir(bounceDir);
            StartCoroutine(IFramesCoroutine(hitInvFramesDuration));
        }
    }

    //El jugador ha muerto
    public void Death(GameObject killer)
    {
        print($"Morio por: {killer.name}");
        Cam.follow = false;
        hasDied = true;

        int dirX = (int)Mathf.Sign(transform.position.x - killer.transform.position.x);
        int multVel = 20;
        Vector2 bounceDir = new Vector2(dirX, 1f).normalized;

        //Lanzar al player
        capsuleCollider.enabled = false;
        playerMoveState = PlayerMoveStates.Death;
        rb.velocity = bounceDir * multVel;
        rb.gravityScale = gravityScale;

        //Desatachar el casco y la espada
        animator.enabled = false;
        Casco.SetParent(transform.parent);
        Espada.SetParent(transform.parent);

        Casco.position = transform.position;
        Espada.position = transform.position;

        Casco.GetComponent<Rigidbody2D>().velocity = new Vector2(dirX * 0.9f, 0.1f) * multVel;
        Espada.GetComponent<Rigidbody2D>().velocity = new Vector2(dirX * 0.1f, 0.9f) * multVel;

        //Rotar al player
        StartCoroutine(DeathCoroutine());

        //Audio
        AudioManager.instance.StopAll();
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.lego_breaking_sfx);
        AudioClip clip = CoolFunctions.PlayRandomClip(MusicLibrary.instance.player_die_sfxs);

        CoolFunctions.InvokeDelayed(this, clip.length - 0.15f, () => { UIManager.instance.ReloadScene(); });
    }

    IEnumerator DeathCoroutine()
    {
        float rotAngle = 30;
        while (true)
        {
            transform.RotateAround(CenterPos.position, Vector3.forward, rotAngle);
            yield return null;
        }
    }

    public void AddForceToDir(Vector2 dir, float mult = 10, float frozenTime = 0.3f)
    {
        canMove = false;
        CoolFunctions.InvokeDelayed(this, frozenTime, () => { canMove = true; });

        rb.velocity = Vector2.zero;
        rb.AddForce(dir.normalized * mult, ForceMode2D.Impulse);
    }

    IEnumerator AttackCoroutine(PlayerMoveStates attackType)
    {
        isAttacking = true;
        playerMoveState = attackType;
        if (attackType == PlayerMoveStates.AttackRanged)
        {
            PlayerBullet bullet = Instantiate(Snowball_bullet);
            bullet.shootDirection = new Vector2(transform.localScale.x > 0 ? 1 : -1, 1);
            bullet.transform.position = SpawnBulletPosition.position;

            CoolFunctions.PlayerShootSFX();
        }
        else
        {
            CoolFunctions.PlayerAttackSFX();
        }

        yield return new WaitForSeconds(attackDuration);
        isAttacking = false;
    }

    IEnumerator IFramesCoroutine(float duration)
    {
        print($"Start iframes: {duration}");
        iFrameCounter = duration;
        Color ogCol;
        Color playerCol = ogCol = sprtRenderer.color;
        float deltaTime = 0.1f;

        while (iFrameCounter > 0)
        {
            playerCol.a = 0.9f;
            sprtRenderer.color = playerCol;
            yield return new WaitForSeconds(deltaTime);

            playerCol.a = 0.5f;
            sprtRenderer.color = playerCol;
            yield return new WaitForSeconds(deltaTime);

            iFrameCounter -= deltaTime * 2;
        }

        print($"No iframes: {iFrameCounter}");
        sprtRenderer.color = ogCol;
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (iFrameCounter <= 0 && !hasDied)
        {
            if (collision.collider.CompareTag("HurtBox") || collision.collider.GetComponent<Enemy>())
            {
                Vector2 colliderPoint = collision.collider.bounds.ClosestPoint(transform.position);

                Vector2 bounceDir = new Vector2(CenterPos.position.x - colliderPoint.x, CenterPos.position.y - colliderPoint.y).normalized;
                Hit(1, bounceDir, collision.collider.gameObject);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody2D>();

        if (CenterPos == null)
            CenterPos = transform.Find("center");

        Vector2 input = localVel / 8;

        //raycasts
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(raycastPosition + (Vector2)transform.right * -0.25f, transform.right * 0.5f);
        Gizmos.DrawRay(raycastPosition + (Vector2)transform.right * 0.1f, transform.up * -1 * ray_groundedDistance);

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(raycastPosition + (Vector2)transform.right * -0.1f, transform.up * -1 * ray_groundAngleDistance);

        //horizontal
        Gizmos.color = localVel.x > 0 ? Color.red : Color.cyan;
        Gizmos.DrawRay(CenterPos.position, transform.right * input.x);
        Gizmos.DrawWireSphere(CenterPos.position + transform.right * input.x, 0.1f);

        //vertical
        Gizmos.color = localVel.y > 0 ? Color.white : Color.yellow;
        Gizmos.DrawRay(CenterPos.position, transform.up * input.y);
        Gizmos.DrawWireSphere(CenterPos.position + transform.up * input.y, 0.1f);

        Gizmos.color = Color.green;
        RaycastHit2D groundCheckCast = Physics2D.Raycast(raycastPosition, transform.up * -1, ray_groundAngleDistance, LayerMask.GetMask("Ground"));
        Gizmos.DrawWireSphere(groundCheckCast.point, 0.05f);
    }
#endif
}
