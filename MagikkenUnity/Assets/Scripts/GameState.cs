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
    public List<Portal> portals;

    public int stageRadius;
    public int currentAxisOffset;

    public BattlePhase currentPhase;

    public GameState(int stageRadiusInUnityUnits)
    {
        players = new Player[2];
        players[0] = new Player(0);
        players[1] = new Player(1);

        projectilesDuel = new List<ProjectileDuel>();
        projectilesField = new List<ProjectileField>();
        portals = new List<Portal>();

        currentPhase = BattlePhase.DUEL_PHASE;

        stageRadius = stageRadiusInUnityUnits * GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
        currentAxisOffset = 0;
    }
    public void AdvanceFrame(long[] inputs)
    {
        if(currentPhase == BattlePhase.DUEL_PHASE) { UpdateDuelPhase(inputs); }
        else { UpdateFieldPhase(inputs); }
    }
    
    #region Duel Phase
    private void UpdateDuelPhase(long[] inputs)
    {
        // Inputs
        for (int p = 0; p < inputs.Length; p++)
        {

            if ((inputs[p] & INPUT_LEFT) != 0 && (inputs[p] & INPUT_RIGHT) == 0)
            {
                players[p].DuelMove(false);
            }
            else if ((inputs[p] & INPUT_RIGHT) != 0 && (inputs[p] & INPUT_LEFT) == 0)
            {
                players[p].DuelMove(true);
            }
            else
            {
                players[p].notAccelerating = true;
                players[p].currentState = PlayerAnimState.IDLE;
            }

            // If player is on the ground
            if (players[p].position.y <= 0)
            {
                if ((inputs[p] & INPUT_UP) != 0)
                {
                    players[p].velocity.y = Player.jumpPower;
                }
            }
        }

        // Movement
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].decayCounter++;

            players[p].position.x += players[p].velocity.x;
            players[p].position.y += players[p].velocity.y;


            // Boundaries / Walls / Floors / Ceiling
            if (players[p].position.y <= 0)
            {
                players[p].position.y = 0;
                players[p].velocity.y = 0;
            }
            if(players[p].position.x <= -stageRadius)
            {
                players[p].position.x = -stageRadius;
                players[p].velocity.x = 0;
            }
            if (players[p].position.x >= stageRadius)
            {
                players[p].position.x = stageRadius;
                players[p].velocity.x = 0;
            }

            // Decay
            if (players[p].velocity.x != 0 && players[p].IsOnGround && players[p].notAccelerating)
            {
                if (players[p].decayCounter % players[p].decayX == 0)
                {
                    players[p].velocity.x = (int)
                        Mathf.MoveTowards(
                        players[p].velocity.x,
                        0f,
                        FRICTION
                        );
                }
            }
            if (players[p].position.y > 0)
            {
                if (players[p].decayCounter % players[p].decayY == 0)
                {
                    players[p].velocity.y -= GRAVITY;
                }
            }
        }

        // Attacks

        // Last state updates
        for (int p = 0; p < inputs.Length; p++)
        {
            if (players[p].position.y > 0) { players[p].currentState = PlayerAnimState.IN_AIR; }
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
    private void UpdateFieldPhase(long[] inputs)
    {
        //Debug.Log("Field phase.");

        // Inputs
        for (int p = 0; p < inputs.Length; p++)
        {
            Vector2Int moveInputs = new Vector2Int();
            if ((inputs[p] & INPUT_LEFT) != 0 && (inputs[p] & INPUT_RIGHT) == 0)
            {
                moveInputs.x = -1;
            }
            else if ((inputs[p] & INPUT_RIGHT) != 0 && (inputs[p] & INPUT_LEFT) == 0)
            {
                moveInputs.x = 1;
            }
            else
            {
                moveInputs.x = 0;
            }
            if ((inputs[p] & INPUT_DOWN) != 0 && (inputs[p] & INPUT_UP) == 0)
            {
                //Debug.Log("Field down input detected");
                moveInputs.y = -1;
            }
            else if ((inputs[p] & INPUT_UP) != 0 && (inputs[p] & INPUT_DOWN) == 0)
            {
                //Debug.Log("Field up input detected");
                moveInputs.y = 1;
            }
            else
            {
                moveInputs.y = 0;
            }
            players[p].FieldMove(moveInputs);
            //Debug.Log("move inputs:");
            //Debug.Log(moveInputs);
            //Debug.Log(players[p].velocityFieldZ);

            // If player is on the ground
            if (players[p].position.y <= 0)
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
            if (players[p].position.y <= 0)
            {
                players[p].position.y = 0;
                players[p].velocity.y = 0;
            }
            while (players[p].FieldCheckOutOfBounds(stageRadius))
            {
                players[p].FieldPullTowardsOrigin();
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
            if (players[p].position.y > 0)
            {
                if (players[p].decayCounter % players[p].decayY == 0)
                {
                    players[p].velocity.y -= GRAVITY;
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
