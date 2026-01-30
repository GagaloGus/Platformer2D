using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterEnemy : Enemy
{
    [Header("Shoot Options")]
    public bool canShoot = false;
    public float shootColdown = 2f;
    public Ammo ammo;

    void Start()
    {
        base.Start();
        ammo = GetComponentInChildren<Ammo>();
    }
    void Update()
    {
        base.Update();
        if (IsTargetInCone(player.transform) && canShoot)
        {
            StartCoroutine(Shoot());
        }
    }

    public IEnumerator Shoot()
    {
        GameObject bullet = ammo.GetObject();
        bullet.transform.position = transform.position;

        canShoot = false;
        StartCoroutine(ReturnBullet(bullet));

        yield return new WaitForSeconds(shootColdown);
        canShoot = true;
    }

    IEnumerator ReturnBullet(GameObject bullet)
    {
        yield return new WaitForSeconds(5f);
        ammo.ReturnObject(bullet);
    }
}
