using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static GameStateConstants;

public static class GameStateConstants
{
    public const int TARGET_FRAMES_PER_SECOND = 60;
    public const int UNITY_TO_GAME_DISTANCE_MULTIPLIER = 100;

    public const int GRAVITY = 4500;
    public const int FRICTION = 1200;

    public const int INPUT_LEFT = (1 << 0);
    public const int INPUT_RIGHT = (1 << 1);
    public const int INPUT_UP = (1 << 2);
    public const int INPUT_DOWN = (1 << 3);
    public const int INPUT_A = (1 << 4);
    public const int INPUT_B = (1 << 5);
}

public enum BattlePhase
{
    DUEL_PHASE,
    FIELD_PHASE
}

[Serializable]
public class GameState
{
    public static Fix64 FIXED_DELTA_TIME = Fix64.One / (Fix64)TARGET_FRAMES_PER_SECOND;
    public Player[] players;
    public List<ProjectileDuel> projectilesDuel; // Erased during transition to Field Phase.
    public List<ProjectileField> projectilesField; // Erased during transition to Duel Phase.

    public Fix64 stageRadius;

    public FixVector3 duelPlaneForward;
    public FixVector3 duelPlaneRight;

    public BattlePhase currentPhase;

    public GameState(int stageRadiusInUnityUnits)
    {
        players = new Player[2];
        players[0] = new Player(0);
        players[1] = new Player(1);

        projectilesDuel = new List<ProjectileDuel>();
        projectilesField = new List<ProjectileField>();

        currentPhase = BattlePhase.DUEL_PHASE;

        stageRadius = new Fix64(stageRadiusInUnityUnits * GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER);
    }
    public void AdvanceFrame(InputSnapshot[] inputs)
    {
        if(currentPhase == BattlePhase.DUEL_PHASE) { UpdateDuelPhase(inputs); }
        else { UpdateFieldPhase(inputs); }
    }
    
    #region Duel Phase
    private void UpdateDuelPhase(InputSnapshot[] inputs)
    {

        // Inputs
        // Declaration of input/intent for state changes, starting attacks/spells
        //      (allows for super freezes, cinematics, etc. to be done easier)
        // Animation state changes, etc.
        for (int p = 0; p < inputs.Length; p++)
        {
            PlayerStateContext ctx = new PlayerStateContext() {
                currentInputs = inputs[p],
                player = players[p],
                gameState = this
            };
            players[p].stateMachine.AdvanceFrame(ctx, currentPhase);
        }
        // Character physics
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].position += players[p].velocity * FIXED_DELTA_TIME;
            if (players[p].position.y > Fix64.Zero)
            {
                players[p].ApplyGravity();
            }
            else
            {
                players[p].ApplyFriction();

                if (players[p].position.y < Fix64.Zero) { players[p].position.y = Fix64.Zero; }
            }
        }
        // Push collisions
        // Wall collisions
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].EnforceStageBounds(stageRadius);
        }
        // Overlap collisions (push overlapping bodies away from wall, towards centre)
        // Character attack/hit checks
        // Reaction state updates
        // Camera update (axis, etc.) (actually for now just skip this step)

        // Reaction state updates
        for (int p = 0; p < inputs.Length; p++)
        {
            // If character is in a neutral state, they automatically turn to face their opponent.
            if (players[p].position.y > Fix64.Zero) {  }
            else { UpdatePlayerFacing(p); }
        }
    }

    public void UpdatePlayerFacing(int pIndex)
    {
        int enemyIndex = (pIndex == 0) ? 1 : 0;
        if (players[enemyIndex].position.x > players[pIndex].position.x) { players[pIndex].facingRight = true; }
        else if (players[enemyIndex].position.x < players[pIndex].position.x) { players[pIndex].facingRight = false; }
    }

    #endregion

    #region Field Phase
    private void UpdateFieldPhase(InputSnapshot[] inputs)
    {
        // Inputs
        // Declaration of input/intent for state changes, starting attacks/spells
        //      (allows for super freezes, cinematics, etc. to be done easier)
        // Animation state changes, etc.
        for (int p = 0; p < inputs.Length; p++)
        {
            PlayerStateContext ctx = new PlayerStateContext() {
                currentInputs = inputs[p],
                player = players[p],
                gameState = this
            };
            players[p].stateMachine.AdvanceFrame(ctx, currentPhase);
        }
        // Character physics
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].position += players[p].velocity * FIXED_DELTA_TIME;
            if (players[p].position.y > Fix64.Zero)
            {
                players[p].ApplyGravity();
            }
            else
            {
                players[p].ApplyFriction();

                if (players[p].position.y < Fix64.Zero) { players[p].position.y = Fix64.Zero; }
            }
        }
        // Push collisions
        // Wall collisions
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].EnforceStageBounds(stageRadius);
        }
        // Overlap collisions (push overlapping bodies away from wall, towards centre)
        // Character attack/hit checks
        // Reaction state updates


        // (above code copied from Duel Phase)


        //Debug.Log("Field phase.");

        // Inputs
        // Animation state changes, starting attacks/spells, etc.
        // Character physics
        // Wall collisions
        // Reaction state updates




        // Old code below //

        // Movement
        //for (int p = 0; p < inputs.Length; p++)
        //{
        //    players[p].decayCounter++;

        //    players[p].position.x += players[p].velocity.x;
        //    players[p].position.y += players[p].velocity.y;
        //    players[p].positionFieldZ += players[p].velocityFieldZ;


        //    // Boundaries / Walls / Floors / Ceiling
        //    if (players[p].position.y <= Fix64.Zero)
        //    {
        //        players[p].position.y = Fix64.Zero;
        //        players[p].velocity.y = Fix64.Zero;
        //    }
        //    while (players[p].FieldCheckOutOfBounds(stageRadius))
        //    {
        //        players[p].FieldPullTowardsOrigin(Fix64.One);
        //    }
        //}


    }
    #endregion

    public void ChangePhase(int attackerIndex)
    {
        if (currentPhase == BattlePhase.DUEL_PHASE) { currentPhase = BattlePhase.FIELD_PHASE; }
        else if (currentPhase == BattlePhase.FIELD_PHASE) {
            currentPhase = BattlePhase.DUEL_PHASE;
            CalibrateDuelPlane();
        }
    }

    public void CalibrateDuelPlane()
    {
        duelPlaneRight = players[1].position - players[0].position;
        duelPlaneRight.y = Fix64.Zero;
        duelPlaneRight = duelPlaneRight.Normalized();

        duelPlaneForward = duelPlaneRight.RotatedAroundYAxis90DegreesCounterclockwise();
    }

    private void ReadInputs() { }
}
