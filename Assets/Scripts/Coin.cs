using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [Range(0,2)] public int coinType = 0;
    public int coinAmount;
    public AudioClip collect_sfx;

    private void Awake()
    {
        CoolFunctions.InvokeDelayed(this, Random.Range(0, 1f), () =>
        {
            GetComponentInParent<Animator>().SetInteger("coin type", coinType);
        });
    }

    public void Collected()
    {
        GameManager.instance.GetCoins(coinAmount);
        AudioManager.instance.PlaySFX2D(collect_sfx);
        Destroy(transform.parent.gameObject);
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
