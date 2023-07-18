using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameStateConstants;

public struct InputSnapshot
{
    public Fix64 moveX;
    public Fix64 moveY;
    public long buttonValues;
}

public class BattleInputManager : MonoBehaviour
{
    [SerializeField]
    MainInputManager mainInputSource;
    [SerializeField]
    Camera p1Camera;
    [SerializeField]
    Camera p2Camera;

    InputSnapshot[] currentInputs;
}
