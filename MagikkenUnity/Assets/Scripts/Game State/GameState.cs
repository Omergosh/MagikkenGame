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

    public const int GRAVITY = 3;
    public const int FRICTION = 5;

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
public struct GameState
{
    //public Player player1;
    //public Player player2;
    public Player[] players;
    public List<ProjectileDuel> projectilesDuel; // Erased during transition to Field Phase.
    public List<ProjectileField> projectilesField; // Erased during transition to Duel Phase.

    public Fix64 stageRadius;
    public int currentAxisOffset;

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
        currentAxisOffset = 0;
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
        for (int p = 0; p < inputs.Length; p++)
        {
            long buttonInputs = inputs[p].buttonValues;

            if ((buttonInputs & INPUT_LEFT) != 0 && (buttonInputs & INPUT_RIGHT) == 0)
            {
                players[p].DuelMove(false);
            }
            else if ((buttonInputs & INPUT_RIGHT) != 0 && (buttonInputs & INPUT_LEFT) == 0)
            {
                players[p].DuelMove(true);
            }
            else
            {
                players[p].notAccelerating = true;
                players[p].currentState = PlayerAnimState.IDLE;
            }

            // If player is on the ground
            if (players[p].position.y <= Fix64.Zero)
            {
                if ((buttonInputs & INPUT_UP) != 0)
                {
                    players[p].velocity.y = (Fix64)Player.jumpPower;
                }
            }
        }

        // Animation state changes, starting attacks/spells, etc.

        // Character physics

        // Movement
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].decayCounter++;

            players[p].position.x += players[p].velocity.x;
            players[p].position.y += players[p].velocity.y;


            // Boundaries / Walls / Floors / Ceiling
            if (players[p].position.y <= Fix64.Zero)
            {
                players[p].position.y = Fix64.Zero;
                players[p].velocity.y = Fix64.Zero;
            }
            if(players[p].position.x <= -stageRadius)
            {
                players[p].position.x = -stageRadius;
                players[p].velocity.x = Fix64.Zero;
            }
            if (players[p].position.x >= stageRadius)
            {
                players[p].position.x = stageRadius;
                players[p].velocity.x = Fix64.Zero;
            }

            // Decay
            if (players[p].velocity.x != Fix64.Zero && players[p].IsOnGround && players[p].notAccelerating)
            {
                players[p].velocity.x = Fix64.Zero;
            }
            if (players[p].position.y > Fix64.Zero)
            {
                if (players[p].decayCounter % players[p].decayY == 0)
                {
                    players[p].velocity.y -= (Fix64)GRAVITY;
                }
            }
        }

        // Wall collisions

        // Reaction state updates
        for (int p = 0; p < inputs.Length; p++)
        {
            // If character is in a neutral state, they automatically turn to face their opponent.
            if (players[p].position.y > Fix64.Zero) { players[p].currentState = PlayerAnimState.IN_AIR; }
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
        //Debug.Log("Field phase.");

        // Inputs
        // Animation state changes, starting attacks/spells, etc.
        // Character physics
        // Wall collisions
        // Reaction state updates

        // Inputs
        for (int p = 0; p < inputs.Length; p++)
        {
            long buttonInputs = inputs[p].buttonValues;
            FixVector2 moveInputs = new FixVector2();
            if ((buttonInputs & INPUT_LEFT) != 0 && (buttonInputs & INPUT_RIGHT) == 0)
            {
                moveInputs.x = -Fix64.One;
            }
            else if ((buttonInputs & INPUT_RIGHT) != 0 && (buttonInputs & INPUT_LEFT) == 0)
            {
                moveInputs.x = Fix64.One;
            }
            else
            {
                moveInputs.x = Fix64.Zero;
            }
            if ((buttonInputs & INPUT_DOWN) != 0 && (buttonInputs & INPUT_UP) == 0)
            {
                //Debug.Log("Field down input detected");
                moveInputs.y = -Fix64.One;
            }
            else if ((buttonInputs & INPUT_UP) != 0 && (buttonInputs & INPUT_DOWN) == 0)
            {
                //Debug.Log("Field up input detected");
                moveInputs.y = Fix64.One;
            }
            else
            {
                moveInputs.y = Fix64.Zero;
            }
            players[p].FieldMove(moveInputs);
            //Debug.Log("move inputs:");
            //Debug.Log(moveInputs);
            //Debug.Log(players[p].velocityFieldZ);

            // If player is on the ground
            if (players[p].position.y <= Fix64.Zero)
            {
                // No ability to jump yet
            }
        }

        // Movement
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].decayCounter++;

            players[p].position.x += players[p].velocity.x;
            players[p].position.y += players[p].velocity.y;
            players[p].positionFieldZ += players[p].velocityFieldZ;


            // Boundaries / Walls / Floors / Ceiling
            if (players[p].position.y <= Fix64.Zero)
            {
                players[p].position.y = Fix64.Zero;
                players[p].velocity.y = Fix64.Zero;
            }
            while (players[p].FieldCheckOutOfBounds(stageRadius))
            {
                players[p].FieldPullTowardsOrigin(Fix64.One);
            }
            //if (players[p].position.x <= -stageRadius)
            //{
            //    players[p].position.x = -stageRadius;
            //    players[p].velocity.x = 0;
            //}
            //if (players[p].position.x >= stageRadius)
            //{
            //    players[p].position.x = stageRadius;
            //    players[p].velocity.x = 0;
            //}

            // Decay
            //if (players[p].velocity.x != 0 && players[p].IsOnGround && players[p].notAccelerating)
            //{
            //    if (players[p].decayCounter % players[p].decayX == 0)
            //    {
            //        players[p].velocity.x = (int)
            //            Mathf.MoveTowards(
            //            players[p].velocity.x,
            //            0f,
            //            FRICTION
            //            );
            //    }
            //}
            if (players[p].position.y > Fix64.Zero)
            {
                if (players[p].decayCounter % players[p].decayY == 0)
                {
                    players[p].velocity.y -= (Fix64)GRAVITY;
                }
            }
        }


    }
    #endregion

    public void ChangePhase()
    {
        if (currentPhase == BattlePhase.DUEL_PHASE) { currentPhase = BattlePhase.FIELD_PHASE; }
        else if (currentPhase == BattlePhase.FIELD_PHASE) { currentPhase = BattlePhase.DUEL_PHASE; }
    }

    private void ReadInputs() { }
}
