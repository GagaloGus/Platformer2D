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
        if (IsTargetInCone(player.transform) && canShoot)
        {
            StartCoroutine(Shoot());
        }
        if (IsTargetInCone(player.transform) == false && canPatrol && PatrolPoints.Count > 0)
        {
            Patrol();
        }

        // Comprueba constantemente si tiene un obstaculo mas alto que él
        CheckWall();
        // Comprueba constantemente si este objeto esta tocando el suelo
        CheckGround();

        // Comprueba si tiene un obstaculo delante, si esta tocando el suelo y si puede saltar dicho obstaculo
        if (wallFound && isGrounded && canJump)
        {
            // LLama el método para que este objeto salte
            Jump();
        }

        // Si deja de ver al objetivo pero ya lo estaba viendo un momento antes llama al metodo para buscarlo
        if (IsTargetInCone(player.transform) == false && wasInside == true)
        {
            // LLama el método para que este objeto siga siguiendo al objetivo durante un tiempo (Como buscandolo)
            Search();
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
