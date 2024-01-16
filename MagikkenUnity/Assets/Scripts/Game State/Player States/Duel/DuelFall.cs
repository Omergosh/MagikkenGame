using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class DuelFall : PlayerState
{
    public override void OnStart(PlayerStateContext context)
    {
        //Debug.Log("duel fall start");
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        //if(DuelCommonTransitions.Common)
        if (context.player.duel2DPosition.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        //Debug.Log("duel fall update");
    }

    public override void OnEnd(PlayerStateContext context)
    {
        //Debug.Log("duel fall end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldFall());
        return true;
    }
}