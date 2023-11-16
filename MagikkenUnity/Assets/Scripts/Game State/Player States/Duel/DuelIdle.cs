using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public class DuelIdle : PlayerState
{
    Fix64 moveDeadZone;

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel idle start");
        moveDeadZone = Fix64.One / new Fix64(20);
        context.player.FaceOtherPlayer(context);
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        if((context.currentInputs.buttonValues & INPUT_A) != 0)
        {
            context.player.stateMachine.SetState(context, new DuelJab());
            return;
        }

        // Check if player will jump
        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        // Check for movement input
        if (Fix64.Abs(context.currentInputs.moveX) > moveDeadZone)
        {
            if (context.currentInputs.moveX < Fix64.Zero)
            {
                // Start moving left
                if (context.player.IsOpponentOnRightDuel(context))
                { context.player.stateMachine.SetState(context, new DuelMoveBackward()); }
                else
                { context.player.stateMachine.SetState(context, new DuelMoveForward()); }
                return;
            }
            else
            {
                // Start moving right
                if (context.player.IsOpponentOnRightDuel(context))
                { context.player.stateMachine.SetState(context, new DuelMoveForward()); }
                else
                { context.player.stateMachine.SetState(context, new DuelMoveBackward()); }
                return;
            }
        }

        //Debug.Log("duel idle update");
        context.player.FaceOtherPlayer(context);
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel idle end");
        Debug.Log(context.player.playerIndex);
        Debug.Log((Vector2)context.gameState.players[0].duel2DPosition);
        Debug.Log((Vector3)context.gameState.players[0].DuelPosition);
        Debug.Log((Vector3)context.gameState.players[0].position3D);
        Debug.Log((Vector2)context.gameState.players[1].duel2DPosition);
        Debug.Log((Vector3)context.gameState.players[1].DuelPosition);
        Debug.Log((Vector3)context.gameState.players[1].position3D);
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldIdle());
        return true;
    }
}