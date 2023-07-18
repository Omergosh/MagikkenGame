public interface PlayerState
{
    //AnimationStateData? animState; // If null, then the same thing happens for every frame of this state's animation.

    public void OnStart(PlayerStateContext stateContext);
    public void OnUpdate(PlayerStateContext stateContext);
    public void OnEnd(PlayerStateContext stateContext);
}