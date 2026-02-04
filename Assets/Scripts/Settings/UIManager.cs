using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    [Range(1, 3)] public uint levelInt = 1;
    public Animator PantallaCarga;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        AudioManager.instance.StopAll();
        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 2);

        if(levelInt == 1)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level1_song);
        else if (levelInt == 2)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level2_song);
        else if (levelInt == 3)
            AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level3_song);
    }

    public void ChangeScene(string sceneName)
    {
        AudioManager.instance.StopAll();
        AudioClip clip = CoolFunctions.PlayRandomClip(MusicLibrary.instance.level_load_sfxs);
        PantallaCarga.gameObject.SetActive(true);
        PantallaCarga.SetInteger("state", 1);
        float rnd = Mathf.Clamp(Random.Range(0.5f, 1.5f) * clip.length / 2, 0, 8);
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
