using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class FieldFall : PlayerState
{
    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("field fall start");
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        // Update variables //


        // State change guard clauses //
        if (context.player.position.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new FieldIdle());
            return;
        }
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("field fall end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new DuelFall());
        return true;
    }
}