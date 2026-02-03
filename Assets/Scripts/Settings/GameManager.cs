using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    public GameObject Prefab_Explosion;
    public int coins;

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

    public void GetCoins(int amount)
    {
        coins += amount;
    }

    public void CreateExplosion(Transform objTransform)
    {
        CreateExplosion(objTransform.position, objTransform.localScale);
    }

    public void CreateExplosion(Vector3 position, Vector3 localScale)
    {
        Transform kaput = Instantiate(Prefab_Explosion).transform;
        kaput.position = position;
        kaput.localScale = localScale;
    }

    public void ChangeScene(string sceneName)
    {
        StartCoroutine(ChangeAsyncScene(sceneName));
    }

    IEnumerator ChangeAsyncScene(string sceneName)
    {
        print($"loading scene async {sceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
