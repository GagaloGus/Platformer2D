using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float bulletSpeed = 10f, dirX = -1f;
    public GameObject parent;

    private void Start()
    {
        parent = GetComponentInParent<Ammo>().gameObject.GetComponentInParent<ShooterEnemy>().gameObject;
        dirX = parent.GetComponent<ShooterEnemy>().dir;
    }
    void Update()
    {
        Vector2 move = new Vector2(dirX, 0f) * bulletSpeed * Time.deltaTime;
        transform.Translate(move);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player" || collision.gameObject.layer == 3)
        {
            gameObject.SetActive(false);
        }
    }
}
