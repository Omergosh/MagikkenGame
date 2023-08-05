using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugVisuals : MonoBehaviour
{
    // References
    [SerializeField] private Transform collisionBoxDisplay;
    [SerializeField] private Transform hurtboxDisplay;
    [SerializeField] private Transform[] hitboxDisplays;

    public int playerIndex;

    // Options / toggles
    public bool displayHurtbox = false;
    public bool displayHitboxes = false;

    public void UpdateBoxes(in GameState gameState)
    {
        // Collision boxes
        // Hurtbox

        // Hitboxes
        ConvertedHitboxData[] hitboxesData = gameState.players[playerIndex].GetHitboxesRelative();
        for (int i = 0; i < hitboxDisplays.Length; i++)
        {
            if (!displayHitboxes) { hitboxDisplays[i].gameObject.SetActive(false); }
            else
            {
                if (i >= hitboxesData.Length) { hitboxDisplays[i].gameObject.SetActive(false); }
                else
                {
                    hitboxDisplays[i].gameObject.SetActive(true);

                    hitboxDisplays[i].localPosition = (Vector3)hitboxesData[i].position / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                    hitboxDisplays[i].localScale = (Vector3)hitboxesData[i].size / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                }
            }
        }
    }

    float ConvertGameDistanceToUnityUnits(float distanceInGameUnits)
    {
        return distanceInGameUnits / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
    }
}
