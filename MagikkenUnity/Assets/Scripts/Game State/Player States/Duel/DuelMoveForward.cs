using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct DuelMoveForward : PlayerState
{
    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel moveforward start");
        context.player.velocity = new FixVector2((Fix64)Player.moveSpeed, Fix64.Zero);
    }

    public void OnUpdate(PlayerStateContext context)
    {
        // State change guard clauses //

        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        if ((context.currentInputs.buttonValues & INPUT_LEFT) == 0 &&
            (context.currentInputs.buttonValues & INPUT_RIGHT) == 0)
        {
            context.player.velocity = new FixVector2(Fix64.Zero, Fix64.Zero);
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        if((context.currentInputs.buttonValues & INPUT_LEFT) != 0)
        {
            context.player.stateMachine.SetState(context, new DuelMoveBackward());
            return;
        }


        // Actual update logic //

        context.player.velocity = new FixVector2((Fix64)Player.moveSpeed, Fix64.Zero);
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel moveforward end");
    }
}