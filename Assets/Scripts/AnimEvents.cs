using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimEvents : MonoBehaviour
{
    public void canIdle()
    {
        GetComponentInParent<Boss>().spawned = true;
    }
}
