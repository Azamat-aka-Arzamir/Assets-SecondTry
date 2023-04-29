using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    private Movement player;
    [SerializeField]
    private int JumpHoldTime=0;
    private System.Action timerEvent = () => { };
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Movement>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timerEvent.Invoke();
    }
    public void GetMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            player.SetInput(context.ReadValue<Vector2>());
        }
        else if (context.phase == InputActionPhase.Canceled)
        {
            player.SetInput(Vector2.zero);
        }
    }
    int jumpTimer;
    public void GetJump(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            jumpTimer = 0;
            timerEvent += CountJump;
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            timerEvent -= CountJump;
            if(jumpTimer!=0) SendJump();
        }
    }
    void CountJump()
    {
        jumpTimer++;
        if (jumpTimer > JumpHoldTime)
        {
            SendJump();
            jumpTimer = 0;
            timerEvent -= CountJump;
        }
    }
    void SendJump()
    {
        float jumpForce = (float)jumpTimer / JumpHoldTime;
        player.Jump(jumpForce);
        Debug.Log("Space Pressed");
    }
    public void GetAttack(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            player.Attack();
        }
    }
}
