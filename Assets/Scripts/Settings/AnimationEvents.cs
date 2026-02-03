using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvents : MonoBehaviour
{
    public void PlaySfx2D(AudioClip clip)
    {
        AudioManager.instance.PlaySFX2D(clip);
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}
