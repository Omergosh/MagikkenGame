using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class FieldIdle : PlayerState
{
    Fix64 moveDeadZone;
    FixVector2 moveVector;

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("field idle start");
        moveVector = new FixVector2();
        moveDeadZone = Fix64.One / new Fix64(20);
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        if ((context.currentInputs.buttonValues & INPUT_A) != 0)
        {
            context.player.stateMachine.SetState(context, new FieldJab());
            return;
        }

        // Update variables //
        moveVector.x = context.currentInputs.moveX;
        moveVector.y = context.currentInputs.moveY;


        // State change guard clauses //
        if (moveVector.Magnitude() > moveDeadZone)
        {
            context.player.stateMachine.SetState(context, new FieldMove());
            return;
        }

        context.player.FaceOtherPlayer(context);
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field idle end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelIdle());
        return true;
    }
}