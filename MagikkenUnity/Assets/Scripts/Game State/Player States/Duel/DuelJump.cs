using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct DuelJump : PlayerState
{
    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel jump start");
        context.player.velocity.y = (Fix64)Player.duelJumpPower;
    }

    public void OnUpdate(PlayerStateContext context)
    {
        //if(DuelCommonTransitions.Common)
        if (context.player.position.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        if (context.player.velocity.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelFall());
            return;
        }

        //Debug.Log("duel jump update");
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel jump end");
    }

    public bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldFall());
        return true;
    }
}