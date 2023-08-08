using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static GameStateConstants;

public class FightManager : MonoBehaviour
{
    bool gameStarted = false;
    bool gamePaused = false;

    public GameState gameState;

    public BattleInputManager battleInputManager;
    public BattleVisualsManager battleVisualsManager;
    //InputSnapshot[] currentInputs;
    bool usedCurrentInputs = false;

    [SerializeField]
    GameObject p1Model;
    [SerializeField]
    GameObject p2Model;
    //[SerializeField]
    //Transform cameraSwivel;
    //[SerializeField]
    //Animator cameraAnimator;

    [SerializeField] CinemachineVirtualCamera vcamDuel; // toggle on/off
    [SerializeField] Transform vcamDuelTargetGroup; // rotate on phase shift into Duel
    [SerializeField] CinemachineVirtualCamera vcamFieldP1; // toggle on/off
    //[SerializeField] CinemachineVirtualCamera vcamFieldP2; // unused until we add split-screen

    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState(5);
        //currentInputs = new InputSnapshot[2];
        battleInputManager.Initialize();

        Debug.Log("Press any key to start.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!gameStarted)
        {
            if (Input.anyKeyDown)
            {
                Debug.Log("Game start!");
                StartGame();
            }
        }
        else if (gamePaused)
        {

        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape)
                //|| Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Backspace)
                )
            {
                Pause();
                return;
            }

            //if (!usedCurrentInputs)
            //{
            PollDebugInputs(); // enables debug command to trigger a phase shift on button press
            battleInputManager.PollCurrentInputs();
            //}

            UpdateCameras();
            if (gameState.currentPhase == BattlePhase.FIELD_PHASE)
            {
                battleInputManager.MakeMoveVectorCameraBased();
            }
        }

        if(battleVisualsManager != null) { battleVisualsManager.UpdateVisuals(); }
    }

    private void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
    }

    private void UpdateCameras()
    {
        if(gameState.currentPhase == BattlePhase.DUEL_PHASE)
        {
            vcamDuelTargetGroup.forward = (Vector3)gameState.duelPlaneForward;
        }
    }

    private void FixedUpdate()
    {
        if (gameStarted && !gamePaused)
        {
            gameState.AdvanceFrame(battleInputManager.currentInputs);
            usedCurrentInputs = true;
        }
    }

    void Pause()
    {
        Debug.Log("Pause.");
        gamePaused = true;
    }

    void PollDebugInputs()
    {
        // Universal/global/debug commands:
        if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            vcamDuelTargetGroup.rotation = Quaternion.AngleAxis(-30f, Vector3.up);
            Debug.Log("Angle -30");
        }
        if (Input.GetKeyDown(KeyCode.Quote))
        {
            vcamDuelTargetGroup.rotation = Quaternion.AngleAxis(+45f, Vector3.up);
            Debug.Log("Angle +45");
        }
        if (Input.GetKeyDown(KeyCode.Return))
        {
            vcamDuelTargetGroup.rotation = Quaternion.AngleAxis(0f, Vector3.up);
            Debug.Log("Angle 0.");
        }
        if (Input.GetKeyDown(KeyCode.Comma))
        {
            vcamDuelTargetGroup.Rotate(Vector3.up, -10f);
            Debug.Log("Angle -");
        }
        if (Input.GetKeyDown(KeyCode.Period))
        {
            vcamDuelTargetGroup.Rotate(Vector3.up, +10f);
            Debug.Log("Angle +");
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Change phase!");
            gameState.ChangePhase(0);

            if (gameState.currentPhase == BattlePhase.FIELD_PHASE)
            {
                Debug.Log("Change to field view!");
                vcamDuel.gameObject.SetActive(false);
                vcamFieldP1.gameObject.SetActive(true);
            }
            else
            {
                Debug.Log("Change to duel view!");
                vcamDuel.gameObject.SetActive(true);
                vcamFieldP1.gameObject.SetActive(false);
            }
        }
    }
}
