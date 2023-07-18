using System;

[Serializable]
public struct StateMachine {
    public PlayerState state;

    public void SetState(PlayerStateContext context, PlayerState newState) {
        state.OnEnd(context);
        state = newState;
        state.OnStart(context);
    }

    public void AdvanceFrame(PlayerStateContext context)
    {
        state.OnUpdate(context);
    }
}