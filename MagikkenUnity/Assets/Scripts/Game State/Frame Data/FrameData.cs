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
public struct ConvertedHitboxData
{
    public FixVector3 position;
    public FixVector3 size;
    public int damageValue;
    public DamageType damageType;
    public bool isMagic;

    public ConvertedHitboxData(HitboxData hitboxData)
    {
        position = new FixVector3(hitboxData.position);
        size = new FixVector3(hitboxData.size);
        damageValue = hitboxData.damageValue;
        damageType = hitboxData.damageType;
        isMagic = hitboxData.isMagic;
    }
}

[Serializable]
public struct HitboxData
{
    public Vector3Int position;
    public Vector3Int size;
    public int damageValue;
    public DamageType damageType;
    public bool isMagic;
}

[Serializable]
public struct FrameData
{
    public HitboxData[] hitboxes;
    public bool IsActive()
    {
        return hitboxes.Length > 0;
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
