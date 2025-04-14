using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        if (movement.isLaunching) return;
        if (movement.previousState == movement.Idle) movement.anim.SetTrigger("IdleJump");
        else if (movement.previousState == movement.Walk || movement.previousState == movement.Run) movement.anim.SetTrigger("RunJump");
    }
    public override void UpdateState(MovementStateManager movement)
    {
        if(movement.jumped && movement.IsGrounded())
        {
            movement.jumped = false;
            if (movement.hzInput == 0 && movement.vrInput == 0) movement.SwiitchState(movement.Idle);
            else if(Input.GetKey(KeyCode.LeftShift)) movement.SwiitchState(movement.Run);
            else movement.SwiitchState(movement.Walk);
        }
    }
}
