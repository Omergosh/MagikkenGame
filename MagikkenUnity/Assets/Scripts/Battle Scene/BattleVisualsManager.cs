using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleVisualsManager : MonoBehaviour
{
    [SerializeField]
    List<MageAnimHandler> mageAnimHandlers;
    FightManager fightManager;

    public static float stageFloorOffset = -2f;

    // Start is called before the first frame update
    void Start()
    {
        mageAnimHandlers[0].otherPlayerModel = mageAnimHandlers[1];
        mageAnimHandlers[1].otherPlayerModel = mageAnimHandlers[0];

        for (int i = 0; i < mageAnimHandlers.Count; i++)
        {
            mageAnimHandlers[i].playerIndex = i;
        }

        fightManager = FindObjectOfType<FightManager>();
    }

    // Update is called once per frame
    //void LateUpdate()
    //{
    //    UpdateVisuals();
    //}

    public void UpdateVisuals()
    {
        for (int i = 0; i < mageAnimHandlers.Count; i++)
        {
            mageAnimHandlers[i].UpdateVisuals(in fightManager.gameState);
        }
    }
}
