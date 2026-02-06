
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

public static class CoolFunctions
{
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
