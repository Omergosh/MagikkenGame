using AYellowpaper.SerializedCollections;
using B83.Unity.Attributes;
using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageType
{
    LIGHT,
    HEAVY
}

[Serializable]
public struct ConvertedHitsphereData
{
    public FixVector3 position;
    public int radius;
    public int damageValue;
    public DamageType damageType;
    public bool isMagic;

    public ConvertedHitsphereData(HitsphereData hitsphereData)
    {
        position = new FixVector3(hitsphereData.position);
        radius = hitsphereData.radius;
        damageValue = hitsphereData.damageValue;
        damageType = hitsphereData.damageType;
        isMagic = hitsphereData.isMagic;
    }
}

[Serializable]
public struct ConvertedHurtsphereData
{
    public FixVector3 position;
    public int radius;

    public ConvertedHurtsphereData(HurtsphereData hurtsphereData)
    {
        position = new FixVector3(hurtsphereData.position);
        radius = hurtsphereData.radius;
    }
}

[Serializable]
public struct HitsphereData
{
    public Vector3Int position;
    public int radius;
    public int damageValue;
    public DamageType damageType;
    public bool isMagic;
}

[Serializable]
public struct HurtsphereData
{
    public Vector3Int position;
    public int radius;
}

[Serializable]
public struct FrameData
{
    public HitsphereData[] hitspheres;
    public HurtsphereData[] hurtspheres;
    public bool IsActive()
    {
        return hitspheres.Length > 0;
    }
}

[Serializable]
[CreateAssetMenu(fileName = "AnimationFrameData", menuName = "ScriptableObjects/AnimationFrameData", order = 1)]
public class AnimationFrameData : ScriptableObject
{
    public int ManaCost = 0;
    public List<FrameData> frames;
    public List<AvailableCancel> availableCancels = new List<AvailableCancel>();

    public bool IsLastFrame(int frameNumber)
    { return frameNumber == frames.Count - 1; }
}

[Serializable]
public class AnimationDataEntry
{
    [MonoScript(typeof(PlayerState))]
    public string playerState;
    [SerializeField] public AnimationFrameData animationData;
}


[CreateAssetMenu(fileName = "AnimationDataLibrary", menuName = "ScriptableObjects/AnimationDataLibrary", order = 1)]
public class AnimationDataLibrary : ScriptableObject
{
    public List<AnimationDataEntry> data;
}
