using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerstates : MonoBehaviour
{
    Animator animator;
    PlayerMoveStates moveStates;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float inputX = Input.GetAxis("Horizontal");

        if(inputX > 0.01f)
        {
            if (Input.GetKey(KeyCode.LeftShift))
                moveStates = PlayerMoveStates.Run;
            else 
                moveStates = PlayerMoveStates.Walk;
        }
        else
            moveStates = PlayerMoveStates.Idle;


            animator.SetInteger("player_states", (int)moveStates);
    }
}
