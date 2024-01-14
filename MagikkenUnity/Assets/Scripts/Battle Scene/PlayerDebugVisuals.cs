using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDebugVisuals : MonoBehaviour
{
    // References
    [SerializeField] private Transform collisionBoxDisplay;
    [SerializeField] private Transform[] hurtsphereDisplays;
    [SerializeField] private Transform[] hitsphereDisplays;

    public int playerIndex;

    // Options / toggles
    public bool displayHurtspheres = false;
    public bool displayHitspheres = false;

    public void UpdateBoxes(in GameState gameState)
    {
        // Collision boxes
        // Hurtbox
        ConvertedHurtsphereData[] hurtspheresData = gameState.players[playerIndex].GetHurtspheresInFront();
        for (int i = 0; i < hurtsphereDisplays.Length; i++)
        {
            if (!displayHurtspheres) { hurtsphereDisplays[i].gameObject.SetActive(false); }
            else
            {
                if (i >= hurtspheresData.Length) { hurtsphereDisplays[i].gameObject.SetActive(false); }
                else
                {
                    hurtsphereDisplays[i].gameObject.SetActive(true);

                    hurtsphereDisplays[i].localPosition = (Vector3)hurtspheresData[i].position / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                    hurtsphereDisplays[i].localScale = Vector3.one * 2f * hurtspheresData[i].radius / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                }
            }
        }

        // Hitboxes
        ConvertedHitsphereData[] hitspheresData = gameState.players[playerIndex].GetHitspheresInFront();
        for (int i = 0; i < hitsphereDisplays.Length; i++)
        {
            if (!displayHitspheres) { hitsphereDisplays[i].gameObject.SetActive(false); }
            else
            {
                if (i >= hitspheresData.Length) { hitsphereDisplays[i].gameObject.SetActive(false); }
                else
                {
                    hitsphereDisplays[i].gameObject.SetActive(true);

                    hitsphereDisplays[i].localPosition = (Vector3)hitspheresData[i].position / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                    hitsphereDisplays[i].localScale = Vector3.one * 2f * hitspheresData[i].radius / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
                }
            }
        }
    }

    float ConvertGameDistanceToUnityUnits(float distanceInGameUnits)
    {
        return distanceInGameUnits / GameStateConstants.UNITY_TO_GAME_DISTANCE_MULTIPLIER;
    }
}
