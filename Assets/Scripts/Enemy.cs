using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public GameObject player;
    public float speed = 5f, dir, raycastDetection, raycastDetectionDown, jumpForce = 6f;
    public bool wallFound = false, isGrounded = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (transform.position.x - player.transform.position.x < 0)
        {

            dir = 1f;

        }

        if (transform.position.x - player.transform.position.x > 0)
        {

            dir = -1f;

        }

        Vector2 move = new Vector2(dir, 0f) * speed * Time.deltaTime;
        transform.Translate(move);
        CheckWall();
        CheckGround();

        if (wallFound && isGrounded)
        {
            Jump();
        }
    }

    void CheckWall()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y);
        if (dir > 0f)
        {
            RaycastHit2D hit = Physics2D.Raycast(enemyPoS, Vector2.right, raycastDetection, LayerMask.GetMask("Ground"));
            wallFound = hit.collider;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, raycastDetection, LayerMask.GetMask("Ground"));
            wallFound = hit.collider;
        }
    }
    void CheckGround()
    {
        Vector2 enemyPoS = new Vector2(transform.position.x, transform.position.y);
        RaycastHit2D hit = Physics2D.Raycast(enemyPoS, Vector2.down, raycastDetectionDown, LayerMask.GetMask("Ground"));
        isGrounded = hit.collider != null;
    }

    void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    }

    private void OnDrawGizmos()
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
    }
}
