using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    public GameState State;
    public int s_playersAlive;
    public int s_zombiesAlive;

    // Subscribe to the GameManager via GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    // Implement a Function
    // private void GameManagerOnGameStateChanged(GameState obj){} <-- will be run everytiem GameState changes
    // unsubscribe ONDestropy with -= instead of +=
    public static event Action<GameState> OnGameStateChanged;

    void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        UpdateGameState(GameState.SelectCharacter);
    }

    [Server]public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.SelectCharacter:
                HandleSelectCharacter();
                break;
            case GameState.Phase1:
                break;
            case GameState.Phase2:
                break;
            case GameState.Phase3:
                break;
            case GameState.Phase4:
                break;
            case GameState.Phase5:
                break;
            case GameState.Victory:
                break;
            case GameState.Lose:
                break;
            default: 
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }
    
    // Such a Method could be Called when the specific State is called
    [Server]private void HandleSelectCharacter()
    {
        
    }
    [Server]private void HandlePhase1()
    {

    }
 
}

public enum GameState
{
        SelectCharacter,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Phase5,
        Victory,
        Lose
}
