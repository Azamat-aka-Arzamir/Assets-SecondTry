using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoatBehavior : MonoBehaviour
{
    Movement movement;
    Routine AI;
    // Start is called before the first frame update
    void Start()
    {
        movement = GetComponent<Movement>();
        AI = new Routine(Random.Range(1, 3), () => { });
        AI.InvokeOnExit(Choose);
        LogComponent.timerEvent += AI;
        AI.Start();
    }

    // Update is called once per frame
    void Choose()
    {
        int random = Random.Range(-1, 2);
        switch (random)
        {
            case 0:
                AI.action = () => { };
                break;
            case 1:
                AI.action = () => movement.input = Vector2.right;
                break;
            case -1:
                AI.action = () => movement.input = Vector2.left;
                break;
        }
        AI.Start();
        AI.InvokeOnExit(Choose);
    }
}
