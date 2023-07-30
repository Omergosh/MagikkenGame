using AYellowpaper.SerializedCollections;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimationSet", menuName = "ScriptableObjects/AnimationSet", order = 1)]
public class PlayerAnimationSet : ScriptableObject
{
    public SerializedDictionary<string, string> playerStateAnimations;
}
