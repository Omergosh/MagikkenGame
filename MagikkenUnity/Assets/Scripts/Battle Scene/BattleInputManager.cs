using Cinemachine;
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
    CinemachineVirtualCamera p1Camera;
    [SerializeField]
    CinemachineVirtualCamera p2Camera;

    public const float directionInputDeadzone = 0.4f;

    //InputAction p1MoveAction;
    //InputAction p1AttackAction;
    //InputAction p1SpecialAction;

    //InputAction p2MoveAction;
    //InputAction p2AttackAction;
    //InputAction p2SpecialAction;

    public InputSnapshot[] currentInputs = new InputSnapshot[2];

    public void Initialize()
    {
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

            // This block is important for command inputs.
            // e.g. differentiating between 5A and 8A in Field Phase.
            if (pConfig.inputData.rawMoveInput.x < -directionInputDeadzone) { input |= INPUT_LEFT; }
            if (pConfig.inputData.rawMoveInput.x > directionInputDeadzone) { input |= INPUT_RIGHT; }
            if (pConfig.inputData.rawMoveInput.y < -directionInputDeadzone) { input |= INPUT_DOWN; }
            if (pConfig.inputData.rawMoveInput.y > directionInputDeadzone) { input |= INPUT_UP; }

            if (pConfig.inputData.isDownButtonA) { input |= INPUT_A; }
            if (pConfig.inputData.isDownButtonB) { input |= INPUT_B; }
            currentInputs[pConfig.playerIndex].buttonValues = input;
        }
        //Debug.Log(currentInputs);
        //Debug.Log(currentInputs[0]);
        //Debug.Log(currentInputs[0].moveX);
        //Debug.Log(currentInputs[0].buttonValues);
        //Debug.Log(currentInputs[1].buttonValues);
    }

    public void MakeMoveVectorCameraBased()
    {
        Vector3 camForward = p1Camera.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();
        Vector3 camRight = p1Camera.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        foreach (PlayerConfiguration pConfig in GlobalInputManager.instance.playerConfigs)
        {
            pConfig.PollInputs();

            Vector3 rawMove = new Vector3(
                (float)currentInputs[pConfig.playerIndex].moveX,
                0f,
                (float)currentInputs[pConfig.playerIndex].moveY
                );

            Vector3 relativeForwardInput = camForward * (float)currentInputs[pConfig.playerIndex].moveY;
            Vector3 relativeRightInput = camRight * (float)currentInputs[pConfig.playerIndex].moveX;

            Vector3 convertedMove = relativeForwardInput + relativeRightInput;

            currentInputs[pConfig.playerIndex].moveX = (Fix64)convertedMove.x;
            currentInputs[pConfig.playerIndex].moveY = (Fix64)convertedMove.z;
        }
    }
}
