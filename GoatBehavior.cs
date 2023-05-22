using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatBehavior : MonoBehaviour
{
    Movement movement;
    Routine AI;
    public bool eating { get; private set; } 
    public bool dying { get; private set; }
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        AI = new Routine(30, () => { });
        AI.InvokeOnExit(Choose);
        LogComponent.timerEvent += AI;
        LogComponent.timerEvent += () =>{ if (!AI.active) Choose(); };
    }

    // Update is called once per frame
    void Choose()
    {
        eating = false;
        int random = Random.Range(-1, 3);
        switch (random)
        {
            case 0:
                AI.action = () => movement.input = Vector2.zero;
                break;
            case 1:
                AI.action = () => movement.input = Vector2.right;
                break;
            case -1:
                AI.action = () => movement.input = Vector2.left;
                break;
            case 2:
                AI.action=()=>movement.input = Vector2.zero;
                eating = true;
                break;
        }
        AI.Start();
        AI.InvokeOnExit(Choose);
    }
}