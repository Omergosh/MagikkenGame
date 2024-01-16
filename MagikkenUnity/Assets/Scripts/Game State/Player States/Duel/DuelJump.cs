using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class DuelJump : PlayerState
{
    public override void OnStart(PlayerStateContext context)
    {
        //Debug.Log("duel jump start");
        context.player.duel2DVelocity.y = (Fix64)Player.duelJumpPower;
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        //if(DuelCommonTransitions.Common)
        if (context.player.position3D.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        if (context.player.velocity3D.y <= Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelFall());
            return;
        }

        //Debug.Log("duel jump update");
    }

    public override void OnEnd(PlayerStateContext context)
    {
        //Debug.Log("duel jump end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldFall());
        return true;
    }
}