using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public int coinAmount;
    public AudioClip collect_sfx;

    public void Collected()
    {
        GameManager.instance.GetCoins(coinAmount);
        AudioManager.instance.PlaySFX2D(collect_sfx);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController))
        {
            Collected();
        }
    }
}
