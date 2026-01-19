using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public bool moveRight;
    public float speed, friction;
    [SerializeField] float currentSpeed;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = Vector2.right * (moveRight ? 1 : -1) * currentSpeed;

        //Reduce la velocidad periodicamente
        currentSpeed = Mathf.MoveTowards(
            currentSpeed, 0, Time.deltaTime * friction
        );

        if (Mathf.Abs(currentSpeed) < 0.1f)
        {
            print("Hola jeff");
            Destroy(gameObject);
        } 
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.GetComponentInParent<PlayerController>())
        {
            print("Hola slenderman");
            Destroy(gameObject) ;
        }
    }
}
