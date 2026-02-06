using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    GameObject sprite;
    Animator animator;
    public bool spawned;
    void Start()
    {
        player = FindAnyObjectByType<PlayerMove>().gameObject;
        rb = GetComponent<Rigidbody2D>();
        objDetector = transform.GetChild(0).gameObject;
        seen = transform.GetChild(1).gameObject;
        lost = transform.GetChild(2).gameObject;
        sprite = transform.GetChild(3).gameObject;
        animator = sprite.GetComponent<Animator>();
        spawned = false;
    }
    protected override void Update()
    {
        restoreRotation();

        if (IsTargetInCone(player.transform) && spawned == true)
        {
            Move(player.transform.position);
        }

        if (IsTargetInCone(player.transform) == false && canPatrol && PatrolPoints.Count > 0)
        {
            Patrol();
        }
        if (IsTargetInCone(player.transform) == false && wasInside == true && spawned == true)
        {
            // LLama el método para que este objeto siga siguiendo al objetivo durante un tiempo (Como buscandolo)
            Search();
        }
    }

    override protected void Move(Vector2 loc)
    {
        // Calcula si el objetivo se encuentra a su derecha
        if (transform.position.x - loc.x < 0)
        {
            // Cambia su dirección de movimiento hacia la derecha
            dir = 1f;
            sprite.GetComponent<SpriteRenderer>().flipX = true;
            // Cambia la ubicación del detector de obstaculos a la derecha
            objDetector.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y + 1);
            // Cambia la rotacion del campo de vision para que mire a su derecha
            coneDirection = 0f;
        }

        // Calcula si el objetivo se encuentra a su izquierda
        if (transform.position.x - loc.x > 0)
        {
            // Cambia su dirección de movimiento hacia la izquierda
            dir = -1f;
            sprite.GetComponent<SpriteRenderer>().flipX = false;
            // Cambia la ubicación del detector de obstaculos a la izquierda
            objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
            // Cambia la rotacion del campo de vision para que mire a su izquierda
            coneDirection = 180f;
        }
        // Una vez visto hacia que direccion moverse, procede a acelerar
        Vector2 move = new Vector2(dir, 0f) * speed * Time.deltaTime;
        transform.Translate(move);
        animator.SetBool("canWalk", true);
    }

    protected void Patrol()
    {
        Vector2 target = PatrolPoints[currentPointIndex];

        if (Mathf.RoundToInt(transform.position.x) != Mathf.RoundToInt(target.x) && isWaiting == false && canJump)
        {
            Move(target);
        }
        else
        {
            StartCoroutine(wait());
            animator.SetBool("canWalk", false);

            currentPointIndex++;

            if (currentPointIndex >= PatrolPoints.Count)
            {
                currentPointIndex = 0;
            }
        }
    }

    protected void Search()
    {
        canPatrol = false;
        // Solo se mueve si no ha llegado a la última posición conocida
        if (Mathf.Abs(transform.position.x - lastLoc.x) > 0.5f && canJump)
        {
            Move(lastLoc);
        }
        else
        {
            animator.SetBool("canWalk", false);
            seen.SetActive(false);
            lost.SetActive(true);
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            wasInside = false;
            timeLeft = 5f;
            canPatrol = true;
            lost.SetActive(false);
        }
    }

    override protected bool IsTargetInCone(Transform target)
    {
        Vector3 dirToTarget = (target.position - transform.position);
        float dist = dirToTarget.magnitude;

        // Detección por proximidad (Círculo 360º)
        if (dist <= detectionRadius)
        {
            canPatrol = false;
            wasInside = true;
            timeLeft = 5f;
            lastLoc = player.transform.position;
            seen.SetActive(true);
            lost.SetActive(false);
            return true;
        }

        // Detección por Cono
        Vector3 baseDirection = Quaternion.Euler(0, 0, coneDirection) * transform.right;
        float angleToTarget = Vector3.Angle(baseDirection, dirToTarget);

        if (angleToTarget < angle / 2.0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, dirToTarget.normalized, rayRange, obstructionMask | targetMask);
            if (hit.collider != null && ((1 << hit.collider.gameObject.layer) & targetMask) != 0)
            {
                canPatrol = false;
                wasInside = true;
                timeLeft = 5f;
                lastLoc = player.transform.position;
                seen.SetActive(true);
                lost.SetActive(false);
                return true;
            }
        }
        Collider2D hitS = Physics2D.OverlapCircle(transform.position, detectionRadius, targetMask);

        if (hitS != null)
        {
            animator.SetBool("spawn", true);
            if (dir == 1f)
            {
                coneDirection = 180;
                sprite.GetComponent<SpriteRenderer>().flipX = false;
                objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
                dir = -1f;
            }
            else if (dir == -1f)
            {
                coneDirection = 0;
                sprite.GetComponent<SpriteRenderer>().flipX = true;
                objDetector.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y + 1);
                dir = 1f;
            }
        }

        if (dist > rayRange)
        {
            return false;
        }
        // Si no se cumple nada de lo anterior devuelve false
        return false;
    }

    private void restoreRotation()
    {
        if (dir == 1f)
        {
            coneDirection = 180;
            sprite.GetComponent<SpriteRenderer>().flipX = false;
            objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
        }
        else if (dir == -1f)
        {
            coneDirection = 0;
            sprite.GetComponent<SpriteRenderer>().flipX = true;
            objDetector.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y + 1);
        }
    }



    protected void OnDrawGizmos()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y - 1.8f);
        Gizmos.color = Color.red;
        if (dir > 0f)
        {
            Gizmos.DrawRay(enemyPoS, Vector2.right * raycastDetection);
        }
        else
        {
            Gizmos.DrawRay(enemyPoS, Vector2.left * raycastDetection);
        }
        Gizmos.DrawRay(enemyPoS, Vector2.down * raycastDetectionDown);

        Gizmos.color = Color.green;

        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Quaternion upRayRotation = Quaternion.AngleAxis(-(angle / 2.0f) + coneDirection, Vector3.forward);
        Quaternion downRayRotation = Quaternion.AngleAxis((angle / 2.0f) + coneDirection, Vector3.forward);

        Vector3 upRayDirection = upRayRotation * transform.right * rayRange;
        Vector3 downRayDirection = downRayRotation * transform.right * rayRange;

        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(transform.position, upRayDirection);
        Gizmos.DrawRay(transform.position, downRayDirection);
        Gizmos.DrawLine(transform.position + downRayDirection, transform.position + upRayDirection);

        if (PatrolPoints.Count > 0)
        {
            for (int i = 0; i < PatrolPoints.Count; i++)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawWireSphere(PatrolPoints[i], 1);
            }
        }
    }
}
