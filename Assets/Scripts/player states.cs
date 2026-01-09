using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerstates : MonoBehaviour
{
    Animator animator;
    public PlayerMoveStates moveStates;
    public bool isRunning, isSliding;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");

        if(inputX > 0.01f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                isRunning = true;
                if (Input.GetKey(KeyCode.S))
                {
                    moveStates = PlayerMoveStates.Slide;
                    isSliding = true;
                }
                else
                {
                    moveStates = PlayerMoveStates.Run;
                    isSliding = false;
                }
            }
            else
            {
                moveStates = PlayerMoveStates.Walk;
                isRunning = false;
                isSliding = false;
            }
        }
        else
        {
            moveStates = PlayerMoveStates.Idle;
        }

        animator.SetInteger("player_states", (int)moveStates);
    }
}
