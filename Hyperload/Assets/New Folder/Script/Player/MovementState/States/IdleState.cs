using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : MovementBaseState
{
    public override void EnterState(MovementStateManager movement)
    {

    }
    public override void UpdateState(MovementStateManager movement)
    {
        if (movement.dir.magnitude > 0.1f)
        {
            if (Input.GetKey(KeyCode.LeftShift)) movement.SwiitchState(movement.Run);
            else movement.SwiitchState(movement.Walk);
        }
        if (Input.GetKeyDown(KeyCode.C)) movement.SwiitchState(movement.Crouch);
        if (Input.GetKeyDown(KeyCode.Space))
        {
            movement.previousState = this;
            movement.SwiitchState(movement.Jump);
        }
    }
}
