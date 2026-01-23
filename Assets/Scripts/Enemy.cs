using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    Vector2 startPos;
    Rigidbody2D rb;
    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetDamage(int dmg)
    {
        health-= dmg;

        if (health <= 0)
        {
            print("ay");
            Destroy(gameObject);
        }
        else
        {
            print("mamon");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackBox"))
        {
            GetDamage(1);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.GetComponent<PlayerBullet>())
        {
            GetDamage(1);
            Destroy(collision.gameObject);
        }
    }
}
