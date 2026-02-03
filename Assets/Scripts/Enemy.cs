using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Enemy : MonoBehaviour
{
    protected GameObject player, objDetector;

    [Header("Enemy Movement")]
    public float speed = 5f;
    public float dir = -1;
    public float jumpForce = 15f;

    [Header("Obstacle Detection")]
    public float raycastDetection = 1;
    public float raycastDetectionDown = 0.5f;

    [Header("Enemy Field of View")]
    public float angle = 30.0f;
    public float rayRange = 10.0f;
    public float coneDirection = 180;
    [Header("Detection Settings")]
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float detectionRadius = 2f;
    [Header("Time for search Player")]
    public float timeLeft = 5f;

    [Header("Actions Check")]
    public bool wallFound = false;
    public bool isGrounded = false;
    public bool canJump = true;

    [Header("Patrol Points")]
    public List<Vector2> PatrolPoints;

    protected Rigidbody2D rb;
    protected bool wasInside = false, isWaiting = false, canPatrol = true;
    protected Vector2 lastLoc;
    protected int currentPointIndex = 0;

    protected void Start()
    {
        player = FindAnyObjectByType<PlayerMove>().gameObject;
        objDetector = transform.GetChild(0).gameObject;
        rb = GetComponent<Rigidbody2D>();
    }

    protected void Update()
    {
        // Si el objetivo a buscar esta en pleno campo de vision procede a moverse hacia dicho objetivo
        // O si el objetivo dejó de verlo pero lo esta buscando (es decir con el wasInside)
        if (IsTargetInCone(player.transform))
        {
            Move(player.transform.position);
        }

        if (IsTargetInCone(player.transform) == false && canPatrol && PatrolPoints.Count > 0)
        {
            Patrol();
        }

        // Comprueba constantemente si tiene un obstaculo mas alto que él
        CheckWall();
        // Comprueba constantemente si este objeto esta tocando el suelo
        CheckGround();

        // Comprueba si tiene un obstaculo delante, si esta tocando el suelo y si puede saltar dicho obstaculo
        if (wallFound && isGrounded && canJump)
        {
            // LLama el método para que este objeto salte
            Jump();
        }

        // Si deja de ver al objetivo pero ya lo estaba viendo un momento antes llama al metodo para buscarlo
        if (IsTargetInCone(player.transform) == false && wasInside == true)
        {
            // LLama el método para que este objeto siga siguiendo al objetivo durante un tiempo (Como buscandolo)
            Search();
        }
    }

    // Método que comprueba si hay algun objeto con el layer = 'Ground', delante de él
    protected void CheckWall()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y);
        // Comprueba hacia que dirección esta mirando este objeto
        if (dir > 0f)
        {
            coneDirection = 0;
            GetComponent<SpriteRenderer>().flipX = true;
            objDetector.transform.position = new Vector2(transform.position.x + 0.9f, transform.position.y + 1);
            // Crea un RayCastHit2D en líenea recta hacia la dirección establecida, que dará true si colisiona con un objeto con layer 'Ground'
            RaycastHit2D hit = Physics2D.Raycast(enemyPoS, Vector2.right, raycastDetection, LayerMask.GetMask("Ground"));
            // Si hace hit establece 'true' a la variable wallFound
            wallFound = hit.collider;
        }
        else
        {
            coneDirection = 180;
            GetComponent<SpriteRenderer>().flipX = false;
            objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
            // Crea un RayCastHit2D en líenea recta hacia la dirección establecida, que devolverá true si colisiona con un objeto con layer 'Ground'
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, raycastDetection, LayerMask.GetMask("Ground"));
            // Si hace hit establece 'true' a la variable wallFound
            wallFound = hit.collider;
        }
    }

    // Método para comprobar si el objeto debajo de este mismo, tiene el layer = 'Ground'
    protected void CheckGround()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y);
        // Crea un RayCastHit2D hacia abajo desde su posición, que devolverá true si colisiona con un objeto con layer 'Ground'
        RaycastHit2D hit = Physics2D.Raycast(enemyPoS, Vector2.down, raycastDetectionDown, LayerMask.GetMask("Ground"));
        // Si hace hit establece 'true' a la variable isGrounded
        isGrounded = hit.collider != null;
    }

    // Método para impulsar al objeto hacia arriba
    protected void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    protected void Move(Vector2 loc)
    {
        // Calcula si el objetivo se encuentra a su derecha
        if (transform.position.x - loc.x < 0)
        {
            // Cambia su dirección de movimiento hacia la derecha
            dir = 1f;
            GetComponent<SpriteRenderer>().flipX = true;
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
            GetComponent<SpriteRenderer>().flipX = false;
            // Cambia la ubicación del detector de obstaculos a la izquierda
            objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
            // Cambia la rotacion del campo de vision para que mire a su izquierda
            coneDirection = 180f;
        }
        // Una vez visto hacia que direccion moverse, procede a acelerar
        Vector2 move = new Vector2(dir, 0f) * speed * Time.deltaTime;
        transform.Translate(move);
    }
    protected void Patrol()
    {
        Vector2 target = PatrolPoints[currentPointIndex];

        if (Mathf.RoundToInt(transform.position.x) != Mathf.RoundToInt(target.x) && isWaiting == false && canJump)
        {
            Move(target);
            //Debug.Log("Moviendo... Enemigo X: " + Mathf.RoundToInt(transform.position.x) + " Target X: " + Mathf.RoundToInt(target.x));
        }
        else
        {
            //Debug.Log("Llegué al punto " + currentPointIndex);
            StartCoroutine(wait());

            currentPointIndex++;

            if (currentPointIndex >= PatrolPoints.Count)
            {
                currentPointIndex = 0;
            }
        }
    }

    protected IEnumerator wait()
    {
        canPatrol = false;
        isWaiting = true;
        yield return new WaitForSeconds(2);
        isWaiting = false;
        canPatrol = true;
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
            Debug.Log("Llegué al último punto visto");
        }

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0)
        {
            wasInside = false;
            timeLeft = 5f;
            canPatrol = true;
            Debug.Log("Búsqueda terminada");
        }
    }

    protected bool IsTargetInCone(Transform target)
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
            Debug.Log(lastLoc);
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
                Debug.Log(lastLoc);
                return true;
            }
        }
        Collider2D hitS = Physics2D.OverlapCircle(transform.position, detectionRadius, targetMask);

        if (hitS != null)
        {
            if (dir == 1f)
            {
                coneDirection = 180;
                GetComponent<SpriteRenderer>().flipX = false;
                objDetector.transform.position = new Vector2(transform.position.x - 0.9f, transform.position.y + 1);
                dir = -1f;
            }
            else if (dir == -1f)
            {
                coneDirection = 0;
                GetComponent<SpriteRenderer>().flipX = true;
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

    // Metodo par ver visualmente en el editor el gizmos de deteccion de obstaculos y suelo
    protected void OnDrawGizmos()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y);
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

        if(PatrolPoints.Count > 0)
        {
            for (int i = 0; i < PatrolPoints.Count; i++)
            {
                Gizmos.color = Color.white;

                Gizmos.DrawWireSphere(PatrolPoints[i], 1);
            }
        }
    }
}