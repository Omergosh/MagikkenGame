using System;
using System.Collections;
using System.Collections.Generic;

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
public struct Player
{
    // Constants
    public const int moveSpeed = 20;
    public const int moveSpeedBack = 15;
    public const int moveSpeedAir = 15;
    public const int moveAccelAir = 3;
    public const int jumpPower = 25;
    public const int decayXDefault = 3;
    public const int decayXWalkBack = 1;
    public const int decayYDefault = 3;

    public const int hurtboxOriginHeight = 30;
    public const int hurtboxOriginRadius = 60;

    // Frame data (hardcoded, universal)
    public const int projectileFireStartup = 5;
    public const int projectileFireDuration = 30;
    public const int projectileFireRecovery = 20;
    public const int parryDuration = 10;
    public const int parryRecovery = 30;
    public const int portalStartup = 15;
    public const int portalRecovery = 15;
    public const int pummelRange = 50;
    public const int pummelStartup = 2;
    public const int pummelRecovery = 25;

    // Attack / maneuver data (hardcoded, universal)
    public const int pummelDamage = 10;

    // Config
    int playerIndex;

    // State
    public int health;

    public PlayerAnimState currentState;
    public int currentAnimFrame;

    public bool facingRight;
    public int posX;
    public int posY;
    public int posSwivel;
    public int velX;
    public int velY;

    public bool notAccelerating;
    public int decayX;
    public int decayY;
    public int decayCounter;

    public int projectileCooldown;
    public int portalCooldown;
    public int activePortalCount;

    public Player(int newPlayerIndex)
    {
        playerIndex = newPlayerIndex;
        health = 50;
        currentState = PlayerAnimState.IDLE;
        currentAnimFrame = 0;

        facingRight = playerIndex == 0 ? true : false;
        posX = playerIndex == 0 ? -200 : 200;
        posY = 0;
        posSwivel = 0;
        velX = playerIndex == 0 ? -20 : 20;
        velY = 0;
        notAccelerating = true;
        decayX = decayXDefault;
        decayY = decayYDefault;
        decayCounter = 0;

        projectileCooldown = 0;
        portalCooldown = 0;

        activePortalCount = 0;
    }

    public void DuelMove(bool movingRight)
    {
        notAccelerating = false;
        if (posY > 0)
        {
            // Moving in midair
            AccelerateX(
                (movingRight ? 1 : -1) * (moveSpeedAir),
                moveAccelAir
                );
            //velX = (movingRight ? 1 : -1) * (moveSpeedAir);
            decayX = decayXDefault; // Doesn't matter/apply yet because velocity decay doesn't occur in midair
        }
        else
        {
            currentState = PlayerAnimState.MOVING;
            if (movingRight == facingRight)
            {
                // Moving in the direction the player is facing
                AccelerateX(FacingMultiplier * (moveSpeed));
                //velX = FacingMultiplier * (moveSpeed);
                decayX = decayXDefault;
            }
            else
            {
                AccelerateX(FacingMultiplier * (-moveSpeedBack / 2), 100);
                //velX = FacingMultiplier * (-moveSpeedBack / 2);
                decayX = decayXWalkBack;
            }
        }
    }

    // Helper methods / properties
    public void AccelerateX(int targetVelX, int accelX = 1)
    {
        if (targetVelX > 0 && targetVelX > velX)
        {
            if (velX < targetVelX)
            {
                velX += accelX;
                if (velX > targetVelX)
                {
                    velX = targetVelX;
                }
            }
        }
        else if(targetVelX < 0 && targetVelX < velX)
        {
            if (velX > targetVelX)
            {
                velX -= accelX;
                if (velX < targetVelX)
                {
                    velX = targetVelX;
                }
            }
        }
    }


    public int FacingMultiplier { get { return facingRight ? 1 : -1; } }
    public bool IsOnGround { get { return posY <= 0; } }
}
