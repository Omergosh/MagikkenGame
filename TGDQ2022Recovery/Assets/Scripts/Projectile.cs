using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct ProjectileDuel
{
    // Constatns
    public const int damage = 3;
    public const int speed = 5;
    public const int hitCircleRadius = 10;

    // State
    public int ownerIndex;
    public bool facingRight;
    public int posX;
    public int posY;

    public ProjectileDuel(int projectorIndex, int newX, int newY, bool shootingRight)
    {
        ownerIndex = projectorIndex;
        facingRight = shootingRight;
        posX = newX;
        posY = newY;
    }
}

[Serializable]
public struct ProjectileField
{
    // Constants
    public const int damage = 3;
    public const int speed = 8;
    public const int hitCircleRadius = 10;

    // State
    public int ownerIndex;
    public int posX;
    public int posY;
    public int posZ;
    public int rotationDegrees;

    public ProjectileField(int projectorIndex, int newX, int newY, int newZ, int newDegrees)
    {
        ownerIndex = projectorIndex;

        posX = newX;
        posY = newY;
        posZ = newZ;
        rotationDegrees = newDegrees;

    }
}
