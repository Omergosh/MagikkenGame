using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct DuelIdle : PlayerState
{
    Fix64 moveDeadZone;

    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel idle start");
        moveDeadZone = Fix64.One / new Fix64(20);
    }

    public void OnUpdate(PlayerStateContext context)
    {

        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        if (Fix64.Abs(context.currentInputs.moveX) > moveDeadZone)
        {
            if (context.currentInputs.moveX < Fix64.Zero)
            {
                context.player.stateMachine.SetState(context, new DuelMoveBackward());
                return;
            }
            else
            {
                context.player.stateMachine.SetState(context, new DuelMoveForward());
                return;
            }
        }

        //Debug.Log("duel idle update");
    }

    public void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel idle end");
    }

    public bool OnPhaseShift(PlayerStateContext context)
    {
        context.player.stateMachine.SetState(context, new FieldIdle());
        return true;
    }
}