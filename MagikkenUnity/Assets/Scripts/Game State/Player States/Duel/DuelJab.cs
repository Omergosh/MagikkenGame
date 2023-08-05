using FixMath.NET;
using System;
using UnityEngine;
using static GameStateConstants;

public class DuelJab : PlayerState
{

    public override void OnStart(PlayerStateContext context)
    {
        Debug.Log("duel jab start");
        //AnimationDataEntry animEntry = GameManager.instance.animationDataLibrary.data.Find(
        //    (a) => a.playerState == nameof(DuelJab)
        //    );
        //animData = animEntry.animationData;
        animData = FetchAnimData(typeof(DuelJab));
        currentFrame = 0;
    }

    public override void OnUpdate(PlayerStateContext context)
    {
        // Error handling //
        if (currentFrame == null) { throw new NullReferenceException("Current frame is null"); }
        if (animData == null) { throw new NullReferenceException("Frame data is null"); }

        // State change guard clauses //
        if (animData.IsLastFrame((int)currentFrame))
        {
            if (context.gameState.currentPhase == BattlePhase.DUEL_PHASE)
            {
                context.player.stateMachine.SetState(context, new DuelIdle());
            }
            else
            {
                context.player.stateMachine.SetState(context, new FieldIdle());
            }
            return;
        }

        // Actual update logic //
        currentFrame++;
    }

    public override void OnEnd(PlayerStateContext context)
    {
        Debug.Log("duel jab end");
    }

    public override bool OnPhaseShift(PlayerStateContext context)
    {
        //context.player.stateMachine.SetState(context, new FieldMove());
        //return true;
        return false;
    }
}