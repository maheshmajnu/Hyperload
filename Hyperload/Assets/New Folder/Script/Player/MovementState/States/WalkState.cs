using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {
        movement.anim.SetBool("Walking", true);
    }
    public override void UpdateState(MovementStateManager movement)
    {
        if (Input.GetKey(KeyCode.LeftShift)) ExitState(movement, movement.Run);
        else if (Input.GetKeyDown(KeyCode.C)) ExitState(movement, movement.Crouch);
        else if (movement.dir.magnitude < 0.1f) ExitState(movement, movement.Idle);

        if (movement.vrInput < 0) movement.currentMoveSpeed = movement.walkBackSpeed;
        else movement.currentMoveSpeed = movement.walkSpeed;
        if (Input.GetKeyDown(KeyCode.Space) && !movement.isLaunching)
        {
            movement.previousState = this;
            movement.SwiitchState(movement.Jump);
        }
    }

    void ExitState(MovementStateManager movement, MovementBaseState state)
    {
        movement.anim.SetBool("Walking", false);
        movement.SwiitchState(state);
    }
}
