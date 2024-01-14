using FixMath.NET;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    // Constants (phase-agnostic)
    public readonly HurtsphereData defaultHurtsphere = new HurtsphereData()
    {
        position = new Vector3Int(0, 100, 0),
        radius = 75,
    };

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
    public FixVector3 position3D;
    public FixVector3 velocity3D;

    // Reference 3D vectors for orientation/movement
    public FixVector3 directionFacing;
    public FixVector3 directionOfOpponent;
    //public FixVector3 positionOfOpponent;

    public FixVector3 directionDuelPlaneForward;

    // Duel phase 2D plane vectors (without Z-axis/depth)
    public FixVector2 duel2DPosition;
    public FixVector2 duel2DVelocity;


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

        position3D = new FixVector3();
        position3D.x = new Fix64(playerIndex == 0 ? -150 : 150);
        position3D.y = Fix64.Zero;
        
        duel2DPosition = new FixVector2();
        duel2DPosition.x = position3D.x;

        velocity3D = new FixVector3();
        velocity3D.x = new Fix64(playerIndex == 0 ? -400 : 400);
        velocity3D.y = Fix64.Zero;
        
        duel2DVelocity = new FixVector2();
        duel2DVelocity.x = velocity3D.x;

        directionFacing = new FixVector3();
        directionFacing.x = new Fix64(playerIndex == 0 ? 1 : -1);

        // Debug
        //directionFacing = new FixVector3(0.5f, 0f, 0.5f).Normalized();
    }


    // Helper methods / properties
    public void EnforceStageBounds(Fix64 stageRadius, BattlePhase currentPhase)
    {
        // If player is out of bounds, pull them back in
        // Stage bounds only enforce position on the x and z axes, not y (vertical)
        //      (this may change if ceilings are added)
        if (CheckOutOfBounds(stageRadius, currentPhase))
        {
            PullIntoBoundsField(stageRadius, currentPhase);
        }
    }

    #region Field functions
    public void ApplyFriction(BattlePhase currentPhase)
    {
        // Only called when the player is on the ground.

        // this 'originalY' stuff should be unnecessary if this function is only called while the player is on the ground
        //Fix64 originalY = velocity.y;

        if (currentPhase == BattlePhase.DUEL_PHASE)
        {
            duel2DVelocity = Fix64Math.MoveTowards(
                duel2DVelocity,
                FixVector2.Zero,
                (Fix64)GameStateConstants.FRICTION * GameState.FIXED_DELTA_TIME
                );
        }
        else
        {
            // Field Phase
            velocity3D = Fix64Math.MoveTowards(
                velocity3D,
                FixVector3.Zero,
                (Fix64)GameStateConstants.FRICTION * GameState.FIXED_DELTA_TIME
                );
        }


        //velocity.y = originalY;
    }

    public void ApplyGravity(BattlePhase currentPhase)
    {
        if (currentPhase == BattlePhase.DUEL_PHASE)
        {
            duel2DVelocity.y -= (Fix64)GameStateConstants.GRAVITY * GameState.FIXED_DELTA_TIME;
        }
        else
        {
            // Field Phase
            velocity3D.y -= (Fix64)GameStateConstants.GRAVITY * GameState.FIXED_DELTA_TIME;
        }
    }

    public bool CheckOutOfBounds(Fix64 stageRadius, BattlePhase currentPhase)
    {
        if(currentPhase == BattlePhase.DUEL_PHASE)
        {
            return Fix64.Abs(duel2DPosition.x) > stageRadius;
        }

        FixVector3 positionExceptY = new FixVector3(position3D.x, Fix64.Zero, position3D.z);
        return positionExceptY.Magnitude() > stageRadius;
    }

    public void PullIntoBoundsField(Fix64 stageRadius, BattlePhase currentPhase)
    {
        if (currentPhase == BattlePhase.DUEL_PHASE)
        {
            duel2DPosition.x = duel2DPosition.x > Fix64.Zero ? stageRadius : -stageRadius;
        }
        else
        {
            // Field Phase
            FixVector3 positionExceptY = new FixVector3(position3D.x, Fix64.Zero, position3D.z);
            //FixVector3 originExceptY = new FixVector3(Fix64.Zero, position.y, Fix64.Zero);
            position3D.x *= (stageRadius / positionExceptY.Magnitude());
            position3D.z *= (stageRadius / positionExceptY.Magnitude());
        }
    }
    #endregion

    #region Duel functions
    public void SwitchToDuelCoordinates()
    {
        FixVector3 rotatedWorldToDuelPosition = FromWorldToDuelSpace(position3D);
        FixVector3 rotatedWorldToDuelVelocity = FromWorldToDuelSpace(velocity3D);
        duel2DPosition = new FixVector2(rotatedWorldToDuelPosition.x, rotatedWorldToDuelPosition.y);
        duel2DVelocity = new FixVector2(rotatedWorldToDuelVelocity.x, rotatedWorldToDuelVelocity.y);
    }

    public void DuelPhaseUpdate3DCoordinates()
    {
        position3D = FromDuel2DToWorldSpace(duel2DPosition);
        velocity3D = FromDuel2DToWorldSpace(duel2DVelocity);
    }

    public FixVector3 FromDuel2DToWorldSpace(FixVector2 originalDuelVector)
    {
        FixVector3 original3DVector = new FixVector3(originalDuelVector.x, originalDuelVector.y, Fix64.Zero);
        return FromDuel3DToWorldSpace(original3DVector);
    }

    public FixVector3 FromDuel3DToWorldSpace(FixVector3 originalDuelVector)
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
                //Debug.Log(playerIndex);
                //Debug.Log(hypotenuse);
                //Debug.Log(adjacent);
                //Debug.Log(x);
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
    public bool IsOnGround { get { return position3D.y <= Fix64.Zero; } }
    #endregion

    #region Helper methods
    public ConvertedHitsphereData[] GetHitspheresInFront()
    {
        HitsphereData[] hitspheresData = stateMachine.GetHitboxes();
        ConvertedHitsphereData[] hitspheresToReturn = new ConvertedHitsphereData[hitspheresData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hitspheresToReturn.Length; i++)
        {
            hitspheresToReturn[i] = new ConvertedHitsphereData(hitspheresData[i]);
        }


        return hitspheresToReturn;
    }

    public ConvertedHitsphereData[] GetHitspheresRelative()
    {
        HitsphereData[] hitspheresData = stateMachine.GetHitboxes();
        ConvertedHitsphereData[] hitspheresToReturn = new ConvertedHitsphereData[hitspheresData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hitspheresToReturn.Length; i++)
        {
            hitspheresToReturn[i] = new ConvertedHitsphereData(hitspheresData[i]);
            
            FixVector3 newHitspherePosition = Forward * hitspheresToReturn[i].position.z;
            newHitspherePosition = newHitspherePosition + (Right * hitspheresToReturn[i].position.x);
            newHitspherePosition.y = hitspheresToReturn[i].position.y;

            hitspheresToReturn[i].position = newHitspherePosition;
        }


        return hitspheresToReturn;
    }

    public ConvertedHitsphereData[] GetHitspheresWorld(BattlePhase currentPhase)
    {
        ConvertedHitsphereData[] hitspheresToReturn = GetHitspheresRelative();

        FixVector3 playerPositionOffset = position3D;

        for (int i = 0; i < hitspheresToReturn.Length; i++)
        {
            hitspheresToReturn[i].position += playerPositionOffset;
        }

        //Debug.Log($"position3d {playerIndex}: {position3D}");
        //Debug.Log($"position3d {playerIndex}: {position3D}");

        return hitspheresToReturn;
    }

    public ConvertedHurtsphereData[] GetHurtspheresInFront()
    {
        HurtsphereData[] hurtspheresData = stateMachine.GetHurtspheres();

        // Guard clause - if frame data doesn't describe any hurtspheres, use the default.
        if (hurtspheresData.Length == 0)
        {
            return new ConvertedHurtsphereData[1] { new ConvertedHurtsphereData(defaultHurtsphere) };
        }

        ConvertedHurtsphereData[] hurtspheresToReturn = new ConvertedHurtsphereData[hurtspheresData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hurtspheresToReturn.Length; i++)
        {
            hurtspheresToReturn[i] = new ConvertedHurtsphereData(hurtspheresData[i]);
        }


        return hurtspheresToReturn;
    }

    public ConvertedHurtsphereData[] GetHurtspheresRelative()
    {
        HurtsphereData[] hurtspheresData = stateMachine.GetHurtspheres();

        // Guard clause - if frame data doesn't describe any hurtspheres, use the default.
        if (hurtspheresData.Length == 0)
        {
            return new ConvertedHurtsphereData[1] { new ConvertedHurtsphereData(defaultHurtsphere) };
        }

        ConvertedHurtsphereData[] hurtspheresToReturn = new ConvertedHurtsphereData[hurtspheresData.Length];

        //float facingRightMultiplier
        //if(stateMachine.currentPhase == BattlePhase.DUEL_PHASE) { }

        for (int i = 0; i < hurtspheresToReturn.Length; i++)
        {
            hurtspheresToReturn[i] = new ConvertedHurtsphereData(hurtspheresData[i]);

            FixVector3 newHurtspherePosition = Forward * hurtspheresToReturn[i].position.z;
            newHurtspherePosition = newHurtspherePosition + (Right * hurtspheresToReturn[i].position.x);
            newHurtspherePosition.y = hurtspheresToReturn[i].position.y;

            hurtspheresToReturn[i].position = newHurtspherePosition;
        }


        return hurtspheresToReturn;
    }

    public ConvertedHurtsphereData[] GetHurtspheresWorld()
    {
        ConvertedHurtsphereData[] hurtspheresToReturn = GetHurtspheresRelative();

        FixVector3 playerPositionOffset = position3D;

        for (int i = 0; i < hurtspheresToReturn.Length; i++)
        {
            hurtspheresToReturn[i].position += playerPositionOffset;
        }


        return hurtspheresToReturn;
    }

    public void UpdateReferenceVectors(PlayerStateContext stateContext)
    {
        int otherPlayerIndex = playerIndex == 0 ? 1 : 0;
        FixVector3 theirPos = stateContext.gameState.players[otherPlayerIndex].position3D;
        directionOfOpponent = (theirPos - position3D).Normalized();
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
        get { return FromWorldToDuelSpace(position3D); }
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
