using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class DuelMoveBackward : PlayerState
{
    Fix64 moveDeadZone;

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel movebackward start");
        moveDeadZone = Fix64.One / new Fix64(20);
        context.player.velocity = new FixVector3(-(Fix64)Player.moveSpeed, Fix64.Zero, Fix64.Zero);
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        // State change guard clauses //

        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        if (Fix64.Abs(context.currentInputs.moveX) <= moveDeadZone)
        {
            context.player.velocity = FixVector3.Zero;
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        if (context.currentInputs.moveX > Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelMoveForward());
            return;
        }


        // Actual update logic //

        context.player.velocity = new FixVector3(-(Fix64)Player.moveSpeed, Fix64.Zero, Fix64.Zero);
        FaceOtherPlayer(context);
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel movebackward end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldMove());
        return true;
    }
}