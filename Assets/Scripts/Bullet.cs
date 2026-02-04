using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f, dirX = -1f;
    public GameObject parent, player;
    private Rigidbody2D rb;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerMove>().gameObject;
        parent = GetComponentInParent<Ammo>().gameObject.GetComponentInParent<ShooterEnemy>().gameObject;
        rb = GetComponent<Rigidbody2D>();
    }
    private void OnEnable()
    {
        float dist = Vector3.Distance(player.transform.position, parent.transform.position);
        Vector3 direction = player.transform.position - parent.transform.position + Vector3.up * (dist / 3);
        direction.Normalize();

        rb.velocity = direction * bulletSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.gameObject.layer == 3)
        {
            gameObject.SetActive(false);
        }
    }
}
