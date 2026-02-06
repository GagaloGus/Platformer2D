using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health;

    Vector2 startPos;
    Rigidbody2D rb;
    SpriteRenderer sprtRenderer;
    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprtRenderer = GetComponent<SpriteRenderer>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerController.instance.transform.position.x <= transform.position.x && sprtRenderer.flipX)
            sprtRenderer.flipX = false;
        else if (PlayerController.instance.transform.position.x > transform.position.x && !sprtRenderer.flipX)
            sprtRenderer.flipX = true;
    }

    void GetDamage(int dmg)
    {
        health -= dmg;

        if (health <= 0)
        {
            print("ay");
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.enemy_kill_sfx);
            AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.enemy_death_sfxs);
            GameManager.instance.CreateExplosion(transform);
            Destroy(gameObject);
        }
        else
        {
            print("mamon");
            AudioManager.instance.PlaySFX2D(MusicLibrary.instance.enemy_ow_sfx);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerAttackBox"))
        {
            if (PlayerController.instance.isSliding)
            {
                PlayerController.instance.onChargeOnEnemy(this);
                GetDamage(PlayerController.instance.dmgChargeAtk);
            }
            else
                GetDamage(PlayerController.instance.dmgMeleeAtk);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.TryGetComponent(out PlayerBullet bullet))
        {
            GetDamage(bullet.damage);
            Destroy(collision.gameObject);
        }
    }
}
