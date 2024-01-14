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
        context.player.UpdateReferenceVectors(context);

        //Debug.Log($"{currentPhase} {battlePhase}");
        if (currentPhase != battlePhase)
        {
            //Debug.Log("state machine phase shift");
            currentPhase = battlePhase;
            if (state.OnPhaseShift(context)) { return; }
        }
        state.OnUpdate(context);
    }

    public HitsphereData[] GetHitboxes()
    {
        HitsphereData[] hitboxes = new HitsphereData[0];
        //if (state == null) { return hitboxes; }
        if (state.animData != null && state.currentFrame != null)
        {
            hitboxes = state.animData.frames[(int)state.currentFrame].hitspheres;
        }
        return hitboxes;
    }

    public HurtsphereData[] GetHurtspheres()
    {
        HurtsphereData[] hurtboxes = new HurtsphereData[0];
        //if (state == null) { return hitboxes; }
        if (state.animData != null && state.currentFrame != null)
        {
            hurtboxes = state.animData.frames[(int)state.currentFrame].hurtspheres;
        }

        // Unnecessary - just let it be an empty array.
        //if (hurtboxes.Length == 0) { return null; }

        return hurtboxes;
    }
}