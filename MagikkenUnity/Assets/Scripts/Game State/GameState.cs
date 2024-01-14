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

    public const int GRAVITY = 6000;
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
        CalibrateDuelPlane();
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

            //players[p].DuelPhaseUpdate3DCoordinates();
        }
        // Character physics
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].duel2DPosition += players[p].duel2DVelocity * FIXED_DELTA_TIME;
            if (players[p].duel2DPosition.y > Fix64.Zero)
            {
                players[p].ApplyGravity(currentPhase);
            }
            else
            {
                players[p].ApplyFriction(currentPhase);

                if (players[p].duel2DPosition.y < Fix64.Zero) { players[p].duel2DPosition.y = Fix64.Zero; }
            }

            players[p].DuelPhaseUpdate3DCoordinates();
        }
        // Push collisions
        // Wall collisions
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].EnforceStageBounds(stageRadius, currentPhase);

            players[p].DuelPhaseUpdate3DCoordinates();
        }
        // Overlap collisions (push overlapping bodies away from wall, towards centre)
        // Character attack/hit checks
        List<AttackHitInfo> attackHitsAgainstP1 = new List<AttackHitInfo>();
        List<AttackHitInfo> attackHitsAgainstP2 = new List<AttackHitInfo>();
        for (int p = 0; p < inputs.Length; p++)
        {
            for (int otherPlayerIndex = 0; otherPlayerIndex < inputs.Length; otherPlayerIndex++)
            {
                // Guard clause for loop
                if(p == otherPlayerIndex) {  continue; }

                ConvertedHitsphereData[] hitspheres = players[p].GetHitspheresWorld(currentPhase);
                ConvertedHurtsphereData[] hurtspheres = players[otherPlayerIndex].GetHurtspheresWorld();
                foreach (ConvertedHitsphereData hitsphere in hitspheres)
                {
                    Debug.Log($"Hitsphere p{p + 1}: {hitsphere.position} {hitsphere.radius}");
                    foreach (ConvertedHurtsphereData hurtsphere in hurtspheres)
                    {

                        // temp debugging
                        Debug.Log($"Hurtsphere p{otherPlayerIndex + 1}: {hurtsphere.position} {hurtsphere.radius}");


                        Fix64 distanceBetweenOrigins = (hitsphere.position - hurtsphere.position).Magnitude();
                        if (distanceBetweenOrigins < new Fix64(hitsphere.radius + hurtsphere.radius))
                        {
                            Debug.Log("attack hit!");
                        }
                    }
                }
            }
        }


        // Reaction state updates

        // Camera update (axis, etc.) (actually for now just skip this step)
        //CalibrateDuelPlane();

        // Reaction state updates
        for (int p = 0; p < inputs.Length; p++)
        {
            // If character is in a neutral state, they automatically turn to face their opponent.
            //if (players[p].position.y > Fix64.Zero) {  }
            //else { UpdatePlayerFacing(p); }
        }
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
            players[p].position3D += players[p].velocity3D * FIXED_DELTA_TIME;
            if (players[p].position3D.y > Fix64.Zero)
            {
                players[p].ApplyGravity(currentPhase);
            }
            else
            {
                players[p].ApplyFriction(currentPhase);

                if (players[p].position3D.y < Fix64.Zero) { players[p].position3D.y = Fix64.Zero; }
            }
        }
        // Push collisions
        // Wall collisions
        for (int p = 0; p < inputs.Length; p++)
        {
            players[p].EnforceStageBounds(stageRadius, currentPhase);
        }
        // Overlap collisions (push overlapping bodies away from wall, towards centre)

        // Character attack/hit checks
        List<AttackHitInfo> attackHitsAgainstP1 = new List<AttackHitInfo>();
        List<AttackHitInfo> attackHitsAgainstP2 = new List<AttackHitInfo>();
        for (int p = 0; p < inputs.Length; p++)
        {
            for (int otherPlayerIndex = 0; otherPlayerIndex < inputs.Length; otherPlayerIndex++)
            {
                // Guard clause for loop
                if (p == otherPlayerIndex) { continue; }

                ConvertedHitsphereData[] hitspheres = players[p].GetHitspheresWorld(currentPhase);
                ConvertedHurtsphereData[] hurtspheres = players[otherPlayerIndex].GetHurtspheresWorld();
                foreach (ConvertedHitsphereData hitsphere in hitspheres)
                {
                    //Debug.Log($"Hitsphere p{p + 1}: {hitsphere.position} {hitsphere.radius}");
                    
                    foreach (ConvertedHurtsphereData hurtsphere in hurtspheres)
                    {
                        //Debug.Log($"Hurtsphere p{otherPlayerIndex + 1}: {hurtsphere.position} {hurtsphere.radius}");

                        Fix64 distanceBetweenOrigins = (hitsphere.position - hurtsphere.position).Magnitude();
                        if (distanceBetweenOrigins < new Fix64(hitsphere.radius + hurtsphere.radius))
                        {
                            Debug.Log("attack hit!");
                        }
                    }
                }
            }
        }

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
        if (currentPhase == BattlePhase.DUEL_PHASE)
        {
            currentPhase = BattlePhase.FIELD_PHASE;
        }
        else if (currentPhase == BattlePhase.FIELD_PHASE)
        {
            currentPhase = BattlePhase.DUEL_PHASE;
            CalibrateDuelPlane();
            // Force an instant recalculation of each player's reference orders -
            // to avoid incorrect/inconsistent logic on frame 1 of Duel Phase.
            foreach (Player player in players)
            {
                player.UpdateReferenceVectors(new PlayerStateContext()
                {
                    player = player,
                    gameState = this
                });
                player.SwitchToDuelCoordinates();
            }
        }
    }

    public void CalibrateDuelPlane()
    {
        duelPlaneRight = players[1].position3D - players[0].position3D;
        duelPlaneRight.y = Fix64.Zero;
        duelPlaneRight = duelPlaneRight.Normalized();

        duelPlaneForward = duelPlaneRight.RotatedAroundYAxis90DegreesCounterclockwise().Normalized();
    }

    private void ReadInputs() { }
}
