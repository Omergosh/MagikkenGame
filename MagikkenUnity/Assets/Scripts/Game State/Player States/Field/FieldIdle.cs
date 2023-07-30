using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct FieldIdle : PlayerState
{
    Fix64 moveDeadZone;
    FixVector2 moveVector;

    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("field idle start");
        moveVector = new FixVector2();
        moveDeadZone = Fix64.One / new Fix64(20);
    }

    public void OnUpdate(PlayerStateContext context)
    {
        // Update variables //
        moveVector.x = context.currentInputs.moveX;
        moveVector.y = context.currentInputs.moveY;


        // State change guard clauses //
        if (moveVector.Magnitude() > moveDeadZone)
        {
            context.player.stateMachine.SetState(context, new FieldMove());
            return;
        }
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field idle end");
    }

    public bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelIdle());
        return true;
    }
}