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

    long[] currentInputs;
    bool usedCurrentInputs = false;

    public float stageFloorOffset = -2f;

    [SerializeField]
    GameObject p1Model;
    [SerializeField]
    GameObject p2Model;
    [SerializeField]
    Transform cameraSwivel;
    [SerializeField]
    Animator cameraAnimator;


    // Start is called before the first frame update
    void Start()
    {
        gameState = new GameState(6);
        currentInputs = new long[2];

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
                || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Backspace)
                )
            {
                Pause();
                return;
            }

            //if (!usedCurrentInputs)
            //{
            PollCurrentInputs();
            UpdateModels();
            //}
        }
    }

    private void StartGame()
    {
        gameStarted = true;
        gamePaused = false;
    }

    private void UpdateModels()
    {
        p1Model.transform.position = new Vector3(
            gameState.players[0].posX / 100f,
            (gameState.players[0].posY / 100f) + stageFloorOffset,
            p1Model.transform.position.z
            );

        p1Model.transform.localScale = new Vector3(
            gameState.players[0].FacingMultiplier,
            p1Model.transform.localScale.y,
            p1Model.transform.localScale.z
            );

        p2Model.transform.position = new Vector3(
            gameState.players[1].posX / 100f,
            (gameState.players[1].posY / 100f) + stageFloorOffset,
            p2Model.transform.position.z
            );

        p2Model.transform.localScale = new Vector3(
            gameState.players[1].FacingMultiplier,
            p2Model.transform.localScale.y,
            p2Model.transform.localScale.z
            );
    }

    private void FixedUpdate()
    {
        if(gameStarted && !gamePaused)
        {
            gameState.AdvanceFrame(currentInputs);
            usedCurrentInputs = true;
        }
    }

    void Pause()
    {
        Debug.Log("Pause.");
        gamePaused = true;
    }

    void PollCurrentInputs()
    {
        // Update input data to reflect current inputs.
        for (int i = 0; i < 2; i++)
        {
            long input = 0;
            if (i == 0)
            {
                if (Input.GetKey(KeyCode.A)) { input |= INPUT_LEFT; }
                if (Input.GetKey(KeyCode.D)) { input |= INPUT_RIGHT; }
                if (Input.GetKey(KeyCode.W)) { input |= INPUT_UP; }
                if (Input.GetKey(KeyCode.S)) { input |= INPUT_DOWN; }
                if (Input.GetKey(KeyCode.Z)) { input |= INPUT_A; }
                if (Input.GetKey(KeyCode.X)) { input |= INPUT_B; }
            }
            else
            {
                if (Input.GetKey(KeyCode.LeftArrow)) { input |= INPUT_LEFT; }
                if (Input.GetKey(KeyCode.RightArrow)) { input |= INPUT_RIGHT; }
                if (Input.GetKey(KeyCode.UpArrow)) { input |= INPUT_UP; }
                if (Input.GetKey(KeyCode.DownArrow)) { input |= INPUT_DOWN; }
                if (Input.GetKey(KeyCode.O)) { input |= INPUT_A; }
                if (Input.GetKey(KeyCode.P)) { input |= INPUT_B; }
            }

            currentInputs[i] = input;
        }

        // Universal/global/debug commands:
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Change phase!");
            gameState.ChangePhase();

            if (gameState.currentPhase == BattlePhase.FIELD_PHASE)
            {
                cameraAnimator.Play("CameraSideToTopView");
                Debug.Log("Change to field view!");
            }
            else
            {
                cameraAnimator.Play("CameraTopToSideView");
                Debug.Log("Change to duel view!");
            }
        }
    }
}
