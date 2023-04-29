using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

[Serializable]
public class AnimatorController : MonoBehaviour
{
    Movement movement;
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
       // animator.states.Add(new State("Attacking", 10, 5, () => movement.attackTimer != 0 || movement.attackQueue,false));
        //animator.states.Add(new State("Attacking", 10, 5, () => movement.attackTimer != 0 || movement.attackQueue, false));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movement.jumpTimer != 0)
        {
            //LogComponent.Write("Preparing to Jump");
        }
        if (movement.attackTimer != 0 || movement.attackQueue)
        {

        }
        if (movement.onGround)
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1)
            {
                if (rb.velocity.x > 0)
                {
                   // LogComponent.Write("Run R");
                }
                else if (rb.velocity.x < 0)
                {
                   // LogComponent.Write("Run L");
                }
            }
            else
            {
                //LogComponent.Write("Idling");
            }
        }
        else
        {
            if (movement.touchWall != 0)
            {
               // LogComponent.Write("Sliding");
            }
            else
            {
                if (rb.velocity.y > 0)
                {
                   // LogComponent.Write("Flying up");
                }
                else
                {
                   // LogComponent.Write("Flying down");
                }
            }
        }
    }
}

