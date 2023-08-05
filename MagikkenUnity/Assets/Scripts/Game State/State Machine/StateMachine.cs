using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StateMachine
{
    public PlayerState state;
    public BattlePhase currentPhase;

    public void SetState(PlayerStateContext context, PlayerState newState)
    {
        state.OnEnd(context);
        state = newState;
        state.OnStart(context);
    }

    public void AdvanceFrame(PlayerStateContext context, BattlePhase battlePhase)
    {
        //Debug.Log($"{currentPhase} {battlePhase}");
        if (currentPhase != battlePhase)
        {
            //Debug.Log("state machine phase shift");
            currentPhase = battlePhase;
            if (state.OnPhaseShift(context)) { return; }
        }
        state.OnUpdate(context);
    }

    public HitboxData[] GetHitboxes()
    {
        HitboxData[] hitboxes = new HitboxData[0];
        //if (state == null) { return hitboxes; }
        if (state.animData != null && state.currentFrame != null)
        {
            hitboxes = state.animData.frames[(int)state.currentFrame].hitboxes;
        }
        return hitboxes;
    }
}