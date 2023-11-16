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
        context.player.duel2DVelocity = new FixVector2((Fix64)Player.duelMoveSpeed, Fix64.Zero);
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        // State change guard clauses //

        if ((context.currentInputs.buttonValues & INPUT_A) != 0)
        {
            context.player.stateMachine.SetState(context, new DuelJab());
            return;
        }

        // Check if player will jump
        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        // Check if player stopped moving
        if (Fix64.Abs(context.currentInputs.moveX) <= moveDeadZone)
        {
            context.player.duel2DVelocity = FixVector2.Zero;
            context.player.stateMachine.SetState(context, new DuelIdle());
            return;
        }

        // Check if player started moving backwards instead
        bool opponentIsOnRight = context.player.IsOpponentOnRightDuel(context);
        if (
            (context.currentInputs.moveX < Fix64.Zero && opponentIsOnRight) ||
            (context.currentInputs.moveX > Fix64.Zero && !opponentIsOnRight)
            )
        {
            context.player.stateMachine.SetState(context, new DuelMoveBackward());
            return;
        }


        // Actual update logic //
        Fix64 facingMultiplier = opponentIsOnRight ? Fix64.One : -Fix64.One;
        FixVector2 duelRelativeMove = new FixVector2(
            (Fix64)Player.duelMoveSpeed * facingMultiplier,
            Fix64.Zero);
        context.player.duel2DVelocity = duelRelativeMove;
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