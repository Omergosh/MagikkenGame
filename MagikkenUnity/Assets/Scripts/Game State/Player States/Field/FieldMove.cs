using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct FieldMove : PlayerState
{
    Fix64 moveDeadZone;
    FixVector2 moveInputVector;
    FixVector3 moveOutputVector;

    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("field move start");
        moveInputVector = new FixVector2();
        moveDeadZone = Fix64.One / new Fix64(20);
    }

    public void OnUpdate(PlayerStateContext context)
    {
        // Update variables //
        moveInputVector.x = context.currentInputs.moveX;
        moveInputVector.y = context.currentInputs.moveY;


        // State change guard clauses //
        if (moveInputVector.Magnitude() <= moveDeadZone)
        {
            context.player.stateMachine.SetState(context, new FieldIdle());
            return;
        }


        // Actual update logic //
        moveOutputVector = new FixVector3(
            moveInputVector.x,
            Fix64.Zero,
            moveInputVector.y
            );
        context.player.velocity = moveOutputVector * (Fix64)Player.fieldMoveSpeed;
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field move end");
    }

    public bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelIdle());
        return true;
    }
}