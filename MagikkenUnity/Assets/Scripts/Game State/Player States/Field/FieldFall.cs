using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct FieldFall : PlayerState
{
    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("field fall start");
    }

    public void OnUpdate(PlayerStateContext context)
    {
        // Update variables //


        // State change guard clauses //
        if (context.player.position.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new FieldIdle());
            return;
        }
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field fall end");
    }

    public bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelFall());
        return true;
    }
}