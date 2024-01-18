using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleUIManager : MonoBehaviour
{
    [SerializeField] private FightManager fightManager;

    //public event EventHandler<HealthChangeEventArgs> OnP1HealthChange;
    //public event EventHandler<HealthChangeEventArgs> OnP2HealthChange;

    //public class HealthChangeEventArgs : EventArgs
    //{
    //    public int hpCurrent;
    //    public int hpMax;
    //}

    public int p1HP;
    public int p1HPMax;
    public int p2HP;
    public int p2HPMax;

    public Slider p1HPSlider;
    public Slider p2HPSlider;

    // Start is called before the first frame update
    void Start()
    {
        p1HPMax = GameStateConstants.DEFAULT_MAX_PLAYER_HEALTH;
        p2HPMax = GameStateConstants.DEFAULT_MAX_PLAYER_HEALTH;
    }

    // Update is called once per frame
    void Update()
    {
        if (fightManager.gameState.players[0].health != p1HP)
        {
            //OnP1HealthChange?.Invoke(this, new HealthChangeEventArgs { hpCurrent = p1HP, hpMax = p1HPMax });
            p1HP = fightManager.gameState.players[0].health;
            p1HPSlider.value = p1HP;
            //p1HPSlider.maxValue = p1HPMax;
        }
        if (fightManager.gameState.players[1].health != p2HP)
        {
            //OnP2HealthChange?.Invoke(this, new HealthChangeEventArgs { hpCurrent = p2HP, hpMax = p2HPMax });
            p2HP = fightManager.gameState.players[1].health;
            p2HPSlider.value = p2HP;
            //p2HPSlider.maxValue = p2HPMax;
        }

        if (p1HP <= 0) { p1HPSlider.gameObject.SetActive(false); }
        if (p2HP <= 0) { p2HPSlider.gameObject.SetActive(false); }
    }

    
}
