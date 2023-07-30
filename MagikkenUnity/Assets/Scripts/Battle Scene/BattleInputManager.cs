using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static GameStateConstants;

[Serializable]
public struct InputSnapshot
{
    public Fix64 moveX;
    public Fix64 moveY;
    public long buttonValues;
}

public class BattleInputManager : MonoBehaviour
{
    //[SerializeField]
    //GlobalInputManager mainInputSource;
    [SerializeField]
    Camera p1Camera;
    [SerializeField]
    Camera p2Camera;

    //InputAction p1MoveAction;
    //InputAction p1AttackAction;
    //InputAction p1SpecialAction;

    //InputAction p2MoveAction;
    //InputAction p2AttackAction;
    //InputAction p2SpecialAction;

    public InputSnapshot[] currentInputs = new InputSnapshot[2];

    public void Initialize()
    {
        //PlayerInput pi1 = GlobalInputManager.instance.playerConfigs[0].playerInput;
        //pi1.SwitchCurrentControlScheme("BattleControls");
        //p1MoveAction = pi1.actions["Move"];
        //p1MoveAction = pi1.actions["Attack"];
        //p1MoveAction = pi1.actions["Special"];

        //PlayerInput pi2 = GlobalInputManager.instance.playerConfigs[1].playerInput;
        //pi2.SwitchCurrentControlScheme("BattleControls");
        //p2MoveAction = pi2.actions["Move"];
        //p2MoveAction = pi2.actions["Attack"];
        //p2MoveAction = pi2.actions["Special"];

        foreach (PlayerConfiguration pConfig in GlobalInputManager.instance.playerConfigs)
        {
            pConfig.playerInput.SwitchCurrentActionMap("BattleControls");
            pConfig.playerInput.currentActionMap.Enable();
        }
    }

    public void PollCurrentInputs()
    {
        foreach (PlayerConfiguration pConfig in GlobalInputManager.instance.playerConfigs)
        {
            pConfig.PollInputs();

            currentInputs[pConfig.playerIndex].moveX = (Fix64)pConfig.inputData.rawMoveInput.x;
            currentInputs[pConfig.playerIndex].moveY = (Fix64)pConfig.inputData.rawMoveInput.y;

            long input = 0;
            if (pConfig.inputData.isDownButtonA) { input |= INPUT_A; }
            if (pConfig.inputData.isDownButtonB) { input |= INPUT_B; }
            currentInputs[pConfig.playerIndex].buttonValues = input;

            //Debug.Log(pConfig.inputData.rawMoveInput);
            //Debug.Log(pConfig.inputData.isDownButtonA);
            //Debug.Log(pConfig.inputData.isDownButtonB);
        }
        //Debug.Log(currentInputs);
        //Debug.Log(currentInputs[0]);
        //Debug.Log(currentInputs[0].moveX);
        //Debug.Log(currentInputs[0].buttonValues);
        //Debug.Log(currentInputs[1].buttonValues);
    }
}
