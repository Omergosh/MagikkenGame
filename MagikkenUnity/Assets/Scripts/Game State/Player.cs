using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerAnimState
{
    IDLE,
    MOVING,
    IN_AIR,
    ATTACKING,

    PARRYING,
    PARRY_RECOVERY,
    PROJECT_STARTUP,
    PROJECT_RECOVERY,
    PORTAL_STARTUP,
    PORTAL_RECOVERY,
    PUMMEL_STARTUP,
    PUMMEL_RECOVERY,

    GETTING_PUMMELED,
    HITSTUN
}

[Serializable]
public class Player
{
    // Constants (Duel-specific)
    public const int duelMoveSpeed = 500;
    public const int duelMoveSpeedBack = 350;
    public const int moveAccelAir = 100;
    public const int duelJumpPower = 1700;

    // Constants (Field-specific)
    public const int fieldMoveSpeed = 650;
    public const int fieldJumpPower = 3000;

    // Config
    public readonly int playerIndex;

    //////////////////////////////////////////////////////////////
    //                          State                           //
    //////////////////////////////////////////////////////////////
    //      (these values need to be deterministically synced   //
    //      across clients for multiplayer netcode to work)     //
    //////////////////////////////////////////////////////////////
    public int health;

    public StateMachine stateMachine;

    // Physics vectors
    public FixVector3 position;
    public FixVector3 velocity;

    // Reference 3D vectors for orientation/movement
    public FixVector3 directionFacing;
    public FixVector3 directionOfOpponent;
    public FixVector3 directionDuelPlaneForward;
    //public FixVector3 positionOfOpponent;

    // TODO: add helper methods to retrieve the 'effective' 2D vectors to use during the Duel Phase.

    //////////////////////////////////////////////////////////////
    // Below this comment is legacy code.                       //
    //////////////////////////////////////////////////////////////

    public bool facingRight;

    public Player(int newPlayerIndex)
    {
        playerIndex = newPlayerIndex;
        health = 50;
        stateMachine.state = new DuelIdle();

        facingRight = playerIndex == 0 ? true : false;
        position = new FixVector3();
        position.x = new Fix64(playerIndex == 0 ? -200 : 200);
        position.y = Fix64.Zero;
        velocity = new FixVector3();
        velocity.x = new Fix64(playerIndex == 0 ? -20 : 20);
        velocity.y = Fix64.Zero;

        directionFacing = new FixVector3();
        directionFacing.x = new Fix64(playerIndex == 0 ? 1 : -1);

        // Debug
        //directionFacing = new FixVector3(0.5f, 0f, 0.5f).Normalized();
    }


    // Helper methods / properties
    public void EnforceStageBounds(Fix64 stageRadius)
    {
        // If player is out of bounds, pull them back in
        // Stage bounds only enforce position on the x and z axes, not y (vertical)
        //      (this may change if ceilings are added)
        if (CheckOutOfBounds(stageRadius))
        {
            PullIntoBoundsField(stageRadius);
        }

        // TODO: change this behaviour for Duel Phase, to maintain alignment with the duel plane.
    }

    #region Field functions
    public void ApplyFriction()
    {
        // Only called when the player is on the ground.

        // this 'originalY' stuff should be unnecessary if this function is only called while the player is on the ground
        //Fix64 originalY = velocity.y;

        velocity = Fix64Math.MoveTowards(
            velocity,
            FixVector3.Zero,
            (Fix64)GameStateConstants.FRICTION * GameState.FIXED_DELTA_TIME
            );

        //velocity.y = originalY;
    }

    public void ApplyGravity()
    {
        velocity.y -= (Fix64)GameStateConstants.GRAVITY * GameState.FIXED_DELTA_TIME;
    }

    public bool CheckOutOfBounds(Fix64 stageRadius)
    {
        FixVector3 positionExceptY = new FixVector3(position.x, Fix64.Zero, position.z);
        return positionExceptY.Magnitude() > stageRadius;
    }

    public void PullIntoBoundsField(Fix64 stageRadius)
    {
        FixVector3 positionExceptY = new FixVector3(position.x, Fix64.Zero, position.z);
        //FixVector3 originExceptY = new FixVector3(Fix64.Zero, position.y, Fix64.Zero);
        position.x *= (stageRadius / positionExceptY.Magnitude());
        position.z *= (stageRadius / positionExceptY.Magnitude());
    }
    #endregion

    #region Duel functions

    public FixVector3 FromDuelToWorldSpace(FixVector3 originalDuelVector)
    {
        // Assumption: all reference vectors are normalized.

        FixVector3 convertedWorldVector;

        FixVector3 relativeForward = directionDuelPlaneForward * originalDuelVector.z;
        FixVector3 relativeRight = DuelPlaneRight * originalDuelVector.x;

        convertedWorldVector = relativeForward + relativeRight;
        convertedWorldVector.y = originalDuelVector.y;

        return convertedWorldVector;
    }

    public FixVector3 FromDuelToWorldSpaceMoreDetail(FixVector3 originalDuelVector)
    {
        // Assumption: all reference vectors are normalized.

        FixVector3 convertedWorldVector;

        FixVector3 relativeForward = directionDuelPlaneForward * originalDuelVector.z;
        FixVector3 relativeRight = DuelPlaneRight * originalDuelVector.x;

        Fix64 magnitudeExceptY = new FixVector3(
            originalDuelVector.x,
            Fix64.Zero,
            originalDuelVector.z
            )
            .Magnitude();

        convertedWorldVector = relativeForward + relativeRight;
        convertedWorldVector.y = Fix64.Zero;
        convertedWorldVector = convertedWorldVector.Normalized() * magnitudeExceptY;
        convertedWorldVector.y = originalDuelVector.y;

        return convertedWorldVector;
    }

    public FixVector3 FromWorldToDuelSpace(FixVector3 originalWorldVector)
    {
        // Assumption: all reference vectors are normalized.

        FixVector3 convertedDuelVector;
        //FixVector3 originalWorldVectorExceptY = new FixVector3(
        //    originalWorldVector.x,
        //    Fix64.Zero,
        //    originalWorldVector.z
        //    );

        Fix64 hypotenuse = originalWorldVector.Magnitude();
        Fix64 adjacent = hypotenuse; // Backup value in case values are too low for us to use math functions without throwing errors.
        Fix64 dotProduct = FixVector3.DotProduct(originalWorldVector, DuelPlaneRight);

        // This if statement avoids an "Attempted to divide by zero" error.
        if (hypotenuse > GameState.FIXED_DELTA_TIME)
        {
            Fix64 x = dotProduct / hypotenuse;
            // If the value used in Fix64 division/Acos is 'outside a specific range' (-1,1),
            // an error will be thrown,
            // so this block is to avoid raising those errors/exceptions.
            // The error occurs if the magnitude of the vectors plugged in are too small,
            // so we set a minimum mandatory magnitude before we use the arc cosine function.
            // If the value is too small, we just pretend it's zero.
            //if (hypotenuse <= Fix64.One)
            //if(hypotenuse > Fix64.Zero)
            if (x > GameState.FIXED_DELTA_TIME || -x > GameState.FIXED_DELTA_TIME)
            {
                Debug.Log(playerIndex);
                Debug.Log(hypotenuse);
                Debug.Log(adjacent);
                Debug.Log(x);
                x = x > Fix64.One ? Fix64.One : x;
                x = x < -Fix64.One ? -Fix64.One : x;
                Fix64 angle = Fix64.Acos(
                    x
                    );

                adjacent = hypotenuse * Fix64.Cos(angle);
            }
        }

        convertedDuelVector = new FixVector3(
            adjacent,
            originalWorldVector.y,
            Fix64.Zero
            );

        return convertedDuelVector;
    }

    public void UpdateDuelFacing()
    {
        facingRight = true;
        if(FromWorldToDuelSpace(directionOfOpponent).x < Fix64.Zero)
        {
            facingRight = false;
        }
    }

    public int FacingMultiplier { get { return facingRight ? 1 : -1; } }
    public bool IsOnGround { get { return position.y <= Fix64.Zero; } }
    #endregion

    #region Helper methods
    public ConvertedHitboxData[] GetHitboxesInFront()
    {
        HitboxData[] hitboxesData = stateMachine.GetHitboxes();
        ConvertedHitboxData[] hitboxesToReturn = new ConvertedHitboxData[hitboxesData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hitboxesToReturn.Length; i++)
        {
            hitboxesToReturn[i] = new ConvertedHitboxData(hitboxesData[i]);
        }


        return hitboxesToReturn;
    }

    public ConvertedHitboxData[] GetHitboxesRelative()
    {
        HitboxData[] hitboxesData = stateMachine.GetHitboxes();
        ConvertedHitboxData[] hitboxesToReturn = new ConvertedHitboxData[hitboxesData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hitboxesToReturn.Length; i++)
        {
            hitboxesToReturn[i] = new ConvertedHitboxData(hitboxesData[i]);
            hitboxesToReturn[i].position.x *= directionFacing.RotatedAroundYAxis90DegreesClockwise().Magnitude();
            hitboxesToReturn[i].position.z *= directionFacing.Magnitude();
        }


        return hitboxesToReturn;
    }

    public ConvertedHitboxData[] GetHitboxesWorld()
    {
        ConvertedHitboxData[] hitboxesToReturn = GetHitboxesRelative();

        FixVector3 playerPositionOffset = position;

        for (int i = 0; i < hitboxesToReturn.Length; i++)
        {
            hitboxesToReturn[i].position += playerPositionOffset;
        }


        return hitboxesToReturn;
    }

    public void UpdateReferenceVectors(PlayerStateContext stateContext)
    {
        int otherPlayerIndex = playerIndex == 0 ? 1 : 0;
        FixVector3 theirPos = stateContext.gameState.players[otherPlayerIndex].position;
        directionOfOpponent = (theirPos - position).Normalized();
        directionDuelPlaneForward = stateContext.gameState.duelPlaneForward;

        // Debug
        if(playerIndex == 0)
        {
            //Debug.Log("reference vector work p1");
            //Debug.Log((Vector3)theirPos);
            //Debug.Log((Vector3)directionOfOpponent);
            //Debug.Log((Vector3)directionDuelPlaneForward);
        }
    }

    public void FaceOtherPlayer(PlayerStateContext stateContext)
    {
        Forward = directionOfOpponent.Normalized();
        if(stateContext.gameState.currentPhase == BattlePhase.DUEL_PHASE)
        {
            UpdateDuelFacing();
        }
    }

    public bool IsOpponentOnRightDuel(PlayerStateContext stateContext)
    {
        bool isP2OnRight = stateContext.gameState.players[0].DuelPosition.x < stateContext.gameState.players[1].DuelPosition.x;
        if (playerIndex == 0) { return isP2OnRight; }
        return !isP2OnRight;
    }

    public FixVector3 DuelPosition
    {
        get { return FromWorldToDuelSpace(position); }
    }

    public FixVector3 DuelPlaneRight
    {
        get { return directionDuelPlaneForward.RotatedAroundYAxis90DegreesClockwise().Normalized(); }
    }

    public FixVector3 Forward
    {
        get
        {
            FixVector3 newForward = directionFacing;
            newForward.y = Fix64.Zero;
            return newForward.Normalized();
        }
        set
        {
            FixVector3 newForward = value;
            newForward.y = Fix64.Zero;
            directionFacing = newForward.Normalized();
        }
    }
    public FixVector3 Right
    {
        get
        {
            FixVector3 newRight = directionFacing.RotatedAroundYAxis90DegreesClockwise();
            newRight.y = Fix64.Zero;
            return newRight.Normalized();
        }
        set
        {
            FixVector3 newRight = value;
            newRight.y = Fix64.Zero;
            directionFacing = newRight.RotatedAroundYAxis90DegreesCounterclockwise().Normalized();
        }
    }
    #endregion
}
