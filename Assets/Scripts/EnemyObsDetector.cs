using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObsDetector : MonoBehaviour
{
    private GameObject parent;
    void Start()
    {
        parent = transform.parent.gameObject;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle") {
            parent.GetComponent<Enemy>().canJump = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Obstacle")
        {
            parent.GetComponent<Enemy>().canJump = true;
        }
    }
}
