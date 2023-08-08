using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class DuelMoveForward : PlayerState
{
    Fix64 moveDeadZone;

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel moveforward start");
        moveDeadZone = Fix64.One / new Fix64(20);
        context.player.velocity = new FixVector3((Fix64)Player.moveSpeed, Fix64.Zero, Fix64.Zero);
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        // State change guard clauses //

        if ((context.currentInputs.buttonValues & INPUT_A) != 0)
        {
            context.player.stateMachine.SetState(context, new DuelJab());
            return;
        }

        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        if (Fix64.Abs(context.currentInputs.moveX) <= moveDeadZone)
        {
            context.player.velocity = FixVector3.Zero;
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        if(context.currentInputs.moveX < Fix64.Zero)
        {
            context.player.stateMachine.SetState(context, new DuelMoveBackward());
            return;
        }


        // Actual update logic //
        FixVector3 duelRelativeMove = new FixVector3((Fix64)Player.moveSpeed, Fix64.Zero, Fix64.Zero);
        context.player.velocity = context.player.FromDuelToWorldSpace(duelRelativeMove);
        context.player.FaceOtherPlayer(context);
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel moveforward end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldMove());
        return true;
    }
}