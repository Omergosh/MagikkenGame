using static GameStateConstants;
public static class DuelCommonTransitions
{
    public static bool CommonJumpTransitions(PlayerStateContext context)
    {
        if ((context.currentInputs.buttonValues & INPUT_UP) != 0)
        {
            context.player.stateMachine.SetState(context, new DuelJump());
            return true;
        }

        return false;
    }

    public static void CommonToNeutralStateTransitions(PlayerStateContext context)
    {
        
    }
}