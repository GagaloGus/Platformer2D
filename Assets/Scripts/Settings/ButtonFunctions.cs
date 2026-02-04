using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonFunctions : MonoBehaviour
{
    public void ChangeScene(string sceneName)
    {
        UIManager.instance.ChangeScene(sceneName);
    }
}
