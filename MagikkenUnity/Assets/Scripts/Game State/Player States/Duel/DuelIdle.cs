using FixMath.NET;
using UnityEngine;
using static GameStateConstants;

public struct DuelIdle : PlayerState
{
    public void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel idle start");
    }

    public void OnUpdate(PlayerStateContext context)
    {
        if (DuelCommonTransitions.CommonJumpTransitions(context)) { return; }

        if ((context.currentInputs.buttonValues & INPUT_LEFT) != 0 ||
            (context.currentInputs.buttonValues & INPUT_RIGHT) != 0)
        {
            if ((context.currentInputs.buttonValues & INPUT_LEFT) != 0)
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
}