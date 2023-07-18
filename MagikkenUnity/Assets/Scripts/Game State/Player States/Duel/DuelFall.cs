using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct DuelFall : PlayerState
{
    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel fall start");
    }

    public void OnUpdate(PlayerStateContext context)
    {
        //if(DuelCommonTransitions.Common)
        if (context.player.position.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        //Debug.Log("duel fall update");
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel fall end");
    }
}