using B83.Unity.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonCommandInput
{
    LEFT,
    RIGHT,
    UP,
    DOWN,
    A,
    B
}

public static class ButtonCommandInputLibrary
{
    public static readonly Dictionary<ButtonCommandInput, int> buttonCommandsToBinary = new Dictionary<ButtonCommandInput, int>()
    {
        {ButtonCommandInput.LEFT, GameStateConstants.INPUT_LEFT},
        {ButtonCommandInput.RIGHT, GameStateConstants.INPUT_RIGHT},
        {ButtonCommandInput.UP, GameStateConstants.INPUT_UP},
        {ButtonCommandInput.DOWN, GameStateConstants.INPUT_DOWN},
        {ButtonCommandInput.A, GameStateConstants.INPUT_A},
        {ButtonCommandInput.B, GameStateConstants.INPUT_B}
    };
}

[CreateAssetMenu(fileName ="MoveCancel", menuName ="ScriptableObjects/AnimationCancel", order = 2)]
public class AvailableCancel : ScriptableObject
{
    public ButtonCommandInput buttonToCancelWith;
    public bool requiresHit = true;
    [MonoScript(typeof(PlayerState))]
    public string moveToCancelInto;
}
