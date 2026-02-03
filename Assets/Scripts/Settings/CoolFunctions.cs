
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public static class CoolFunctions
{
    public static void PlayerAttackSFX()
    {
        PlayRandomClip(MusicLibrary.instance.player_attack_sfxs);
    }

    public static void PlayerShootSFX()
    {
        PlayRandomClip(MusicLibrary.instance.player_shoot_sfxs);
    }

    public static void EnemyDeathSFX()
    {
        PlayRandomClip(MusicLibrary.instance.enemy_death_sfxs);
    }

    public static void PlayRandomClip(AudioClip[] clips)
    {
        AudioClip clip = clips[Random.Range(0, clips.Length)];

        AudioManager.instance.PlaySFX2D(clip);
    }

    public static void InvokeDelayed(MonoBehaviour m, float delayTime, System.Action f)
    {
        if (f != null)
            m.StartCoroutine(InvokeDelayedCoroutine(delayTime, f));
    }

   static IEnumerator InvokeDelayedCoroutine(float delayTime, System.Action f)
    {
        yield return new WaitForSeconds(delayTime);
        f();
    }
}
