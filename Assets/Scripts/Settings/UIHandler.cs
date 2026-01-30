using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIHandler : MonoBehaviour
{
    public Vector2 spawnEnemyPos;
    public GameObject Prefab_enemy;


    public void SpawnNewEnemy()
    {
        Transform enemy = Instantiate(Prefab_enemy).transform;
        enemy.position = spawnEnemyPos;
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.spawn_sfx);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(spawnEnemyPos, 0.5f);
    }
#endif
}
