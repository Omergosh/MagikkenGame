using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class FieldMove : PlayerState
{
    Fix64 moveDeadZone;
    FixVector2 moveInputVector;
    FixVector3 moveOutputVector;

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("field move start");
        moveInputVector = new FixVector2();
        moveDeadZone = Fix64.One / new Fix64(20);
    }

    public override void OnUpdate(PlayerStateContext context)
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
        context.player.velocity3D = moveOutputVector * (Fix64)Player.fieldMoveSpeed;
        context.player.Forward = moveOutputVector;
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field move end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelIdle());
        return true;
    }
}