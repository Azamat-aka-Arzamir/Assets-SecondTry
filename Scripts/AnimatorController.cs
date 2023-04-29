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
    }
}

