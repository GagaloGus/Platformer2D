using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicLibrary : MonoBehaviour
{
    public static MusicLibrary instance;

    [Header("-------- CANCIONES --------")]
    [Header("Canciones de mundo")]
    public AudioClip level1_song;
    public AudioClip level2_song;
    public AudioClip level3_song;

    [Header("-------- SOUND EFFECTS --------")]
    [Header("player sfx")]
    public AudioClip[] player_attack_sfxs;
    public AudioClip[] player_shoot_sfxs;
    public AudioClip player_crouching_sfx, player_slide_sfx, player_bump_sfx, player_kill_sfx;
    public AudioClip[] player_die_sfxs;

    [Header("enemy sfx")]
    public AudioClip[] enemy_death_sfxs;
    public AudioClip enemy_ow_sfx, enemy_kill_sfx;

    [Header("level sfx")]
    public AudioClip[] level_load_sfxs;

    [Header("otros sfx")]
    public AudioClip[] explosion_sfxs;
    public AudioClip confetti_sfx;
    public AudioClip lego_breaking_sfx;
    public AudioClip spawn_sfx;
    public AudioClip BIG_spawn_sfx;
    public AudioClip switch_on_sfx;
    public AudioClip coin_sfx;
    public AudioClip powerup_sfx;

    private void Awake()
    {
        instance = this;
    }
}
