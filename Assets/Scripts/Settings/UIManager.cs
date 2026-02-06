using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Range(1, 3)] public uint levelInt = 1;

    Animator PantallaCarga;

    private void Awake()
    {
        instance = this;
        PantallaCarga = transform.Find("Pantalla carga").GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.StopAll();
        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 2);
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.door_close_sfx);

        if (levelInt == 1)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level1_song);
        else if (levelInt == 2)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level2_song);
        else if (levelInt == 3)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level3_song);
    }

    public void ChangeScene(string sceneName)
    {
        AudioManager.instance.StopAll();
        AudioManager.instance.PlaySFX2D(MusicLibrary.instance.door_open_sfx);

        bool playLoadClip = Random.value < GameManager.instance.chancePlayLoadSound;
        float clipLength = playLoadClip ? AudioManager.instance.PlayRandomSFX2D(MusicLibrary.instance.level_load_sfxs).length : 1;

        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 1);
        float rnd = Mathf.Clamp(Random.Range(0.4f, 1f) * clipLength, 0, 8);
        print(rnd);

        CoolFunctions.InvokeDelayed(this, rnd, () =>
        {
            GameManager.instance.ChangeScene(sceneName);
        });
    }

    public void ReloadScene()
    {
        ChangeScene(SceneManager.GetActiveScene().name);
    }
}
