using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

[Serializable]
public class PlayerConfiguration
{
    public int playerIndex;
    public PlayerInput playerInput;
    public PlayerDeviceInputData inputData;
    public Camera camera;

    public PlayerConfiguration(PlayerInput pi)
    {
        playerIndex = pi.playerIndex;
        playerInput = pi;
    }

    public void PollInputs()
    {
        inputData.rawMoveInput = playerInput.actions["Move"].ReadValue<Vector2>();
        inputData.isDownButtonA = playerInput.actions["Attack"].IsPressed();
        inputData.isDownButtonB = playerInput.actions["Special"].IsPressed();
        //Debug.Log(playerInput.actions["Move"].ReadValue<Vector2>());
    }
}

[Serializable]
public struct PlayerDeviceInputData
{
    public Vector2 rawMoveInput;
    public bool isDownButtonA;
    public bool isDownButtonB;
}

public class GlobalInputManager : MonoBehaviour
{
    /// <summary>
    /// Singleton.
    /// Responsible for collecting raw data from the input devices of all local players.
    /// Other classes reference this singleton in order to process inputs.
    /// </summary>

    [SerializeField]
    private GameObject playerConfigPrefab;

    [SerializeField]
    public List<PlayerConfiguration> playerConfigs = new List<PlayerConfiguration>();

    public static GlobalInputManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Error, SINGLETON - Trying to create another instance of a singleton!");
            Destroy(this.gameObject); // Someday, this will cause a bug, I'm sure.
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);



            if (SceneManager.GetActiveScene().name == "FightScene")
            {
                DebugInitializeInFightScene();
            }
        }
    }

    private void DebugInitializeInFightScene()
    {
        PlayerInput pi1 = PlayerInput.Instantiate(
            playerConfigPrefab,
            0,
            controlScheme: "Keyboard",
            pairWithDevice: Keyboard.current
            );
        PlayerInput pi2 = PlayerInput.Instantiate(
            playerConfigPrefab,
            1,
            controlScheme: "Gamepad",
            pairWithDevice: Gamepad.current
            );
    }

    public void OnPlayerJoined(PlayerInput pi)
    {
        playerConfigs.Add(new PlayerConfiguration(pi));
        pi.currentActionMap.Enable();
        Debug.Log("message received: a player joined");
    }
}
