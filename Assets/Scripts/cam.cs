using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cam : MonoBehaviour
{
    public bool follow;

    private void Start()
    {
        follow = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(follow)
            transform.position = new Vector3(PlayerController.instance.transform.position.x, PlayerController.instance.transform.position.y, transform.position.z);
    }
}
