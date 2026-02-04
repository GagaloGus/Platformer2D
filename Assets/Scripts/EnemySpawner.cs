using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public KeyCode spawnEnemyKey = KeyCode.F, spawnBossEnemyKey = KeyCode.G;
    public Vector2 spawnEnemyPos;
    public GameObject Prefab_enemy, Prefab_bossEnemy;


    private void Update()
    {
        if (Input.GetKeyDown(spawnEnemyKey) && Prefab_enemy != null)
            SpawnNewEnemy();

        if (Input.GetKeyDown(spawnBossEnemyKey) && Prefab_bossEnemy != null)
            SpawnBossEnemy();
    }

    public void SpawnNewEnemy()
    {
        Transform enemy = Instantiate(Prefab_enemy).transform;
        enemy.position = spawnEnemyPos;
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.spawn_sfx);
    }

    public void SpawnBossEnemy()
    {
        Transform enemy = Instantiate(Prefab_bossEnemy).transform;
        enemy.position = spawnEnemyPos;
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.BIG_spawn_sfx);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnEnemyPos, 0.5f);
    }
#endif
}
