using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public Vector2 shootDirection;
    public float speed;
    public int damage = 1;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        //currentSpeed = speed;
    }

    private void Start()
    {
        rb.velocity = shootDirection * speed;
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs(rb.velocity.magnitude) < 0.5f)
        {
            print("Hola jeff");
            Destroy(gameObject);
        } 
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Vector2 velPos = (Vector2)transform.position + rb.velocity / 3;
        Gizmos.DrawRay(transform.position, rb.velocity / 3);
        Gizmos.DrawWireCube(velPos, Vector3.one / 2);
    }
#endif
}
