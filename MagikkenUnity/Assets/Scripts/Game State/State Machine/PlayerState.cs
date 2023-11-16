using UnityEngine;

public abstract class PlayerState
{
#nullable enable
    public AnimationFrameData? animData; // If null, then the same thing happens for every frame of this state's animation.
    public int? currentFrame;
#nullable disable

    public abstract void OnStart(PlayerStateContext stateContext);
    public abstract void OnUpdate(PlayerStateContext stateContext);
    public abstract void OnEnd(PlayerStateContext stateContext);
    public abstract bool OnPhaseShift(PlayerStateContext stateContext);

    public static AnimationFrameData FetchAnimData(System.Type animState)
    {
        AnimationDataEntry animEntry = GameManager.instance.animationDataLibrary.data.Find(
            (a) => a.playerState == animState.Name
            );
        Debug.Log(animState.Name);
        return (animEntry.animationData);
    }

    //public void SwitchToDuelPhaseCoordinates(PlayerStateContext stateContext)
    //{
    //    stateContext.player.SwitchToDuelCoordinates();
    //}
}