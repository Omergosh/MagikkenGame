using FixMath.NET;
using static GameStateConstants;
public static class DuelCommonTransitions
{
    static Fix64 moveDeadZone = Fix64.One / new Fix64(20);

    public static bool CommonJumpTransitions(PlayerStateContext context)
    {
        //if (context.currentInputs.moveY > moveDeadZone)
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