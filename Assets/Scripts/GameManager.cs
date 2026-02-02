using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject Prefab_Explosion;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
        AudioManager.instance.PlayAmbientMusic(MusicLibrary.instance.level1_song);
    }

    public void CreateExplosion(Transform objTransform)
    {
        Transform kaput = Instantiate(Prefab_Explosion).transform;
        kaput.position = objTransform.position;
        kaput.localScale = objTransform.localScale;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        print($"loaded scene {sceneName}");
    }
}
