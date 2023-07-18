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
    public const int moveSpeed = 400;
    public const int moveSpeedBack = 15;
    public const int moveAccelAir = 100;
    public const int duelJumpPower = 1500;

    // Constants (Field-specific)
    public const int fieldMoveSpeed = 550;
    public const int fieldJumpPower = 2000;

    // Config
    int playerIndex;

    //////////////////////////////////////////////////////////////
    //                          State                           //
    //////////////////////////////////////////////////////////////
    //      (these values need to be deterministically synced   //
    //      across clients for multiplayer netcode to work)     //
    //////////////////////////////////////////////////////////////
    public int health;

    public StateMachine stateMachine;

    //public FixVector3 position;
    //public FixVector3 velocity;
    // TODO: add helper methods to retrieve the 'effective' 2D vectors to use during the Duel Phase.

    // Below this comment is legacy code.                       //
    //////////////////////////////////////////////////////////////

    public PlayerAnimState currentState;
    public int currentAnimFrame;

    public bool facingRight;
    public int posSwivel;

    public bool notAccelerating = true;

    // Physics vectors
    public FixVector2 position;
    public FixVector2 velocity;
    public Fix64 positionFieldZ;
    public Fix64 velocityFieldZ;

    public Player(int newPlayerIndex)
    {
        playerIndex = newPlayerIndex;
        health = 50;
        stateMachine.state = new DuelIdle();

        facingRight = playerIndex == 0 ? true : false;
        position = new FixVector2();
        position.x = new Fix64(playerIndex == 0 ? -200 : 200);
        position.y = Fix64.Zero;
        posSwivel = 0;
        positionFieldZ = Fix64.Zero;
        velocity = new FixVector2();
        velocity.x = new Fix64(playerIndex == 0 ? -20 : 20);
        velocity.y = Fix64.Zero;
        velocityFieldZ = Fix64.Zero;
    }


    // Helper methods / properties
    public void EnforceStageBounds(Fix64 stageRadius)
    {
        // If player is out of bounds, pull them back in
        // Stage bounds only enforce position on the x and z axes, not y (vertical)
        //      (this may change if ceilings are added)
        if (CheckOutOfBounds(stageRadius))
        {
            PullIntoBounds(stageRadius);
        }
    }

    #region Field functions
    public void ApplyFriction()
    {
        // Only called when the player is on the ground.

        // this 'originalY' stuff should be unnecessary if this function is only called while the player is on the ground
        //Fix64 originalY = velocity.y;

        velocity = Fix64Math.MoveTowards(
            velocity,
            FixVector2.Zero,
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
        //FixVector3 positionExceptY = new FixVector3(position.x, Fix64.Zero, position.z);
        FixVector3 positionExceptY = new FixVector3(position.x, Fix64.Zero, positionFieldZ);
        return positionExceptY.Magnitude() > stageRadius;
    }

    public void PullIntoBounds(Fix64 stageRadius)
    {
        FixVector3 positionExceptY = new FixVector3(position.x, Fix64.Zero, positionFieldZ);
        //FixVector3 originExceptY = new FixVector3(Fix64.Zero, position.y, Fix64.Zero);
        position.x *= (stageRadius / positionExceptY.Magnitude());
        positionFieldZ *= (stageRadius / positionExceptY.Magnitude());
    }
    #endregion

    #region Duel functions



    public int FacingMultiplier { get { return facingRight ? 1 : -1; } }
    public bool IsOnGround { get { return position.y <= Fix64.Zero; } }
    #endregion
}
