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

    // In Awake Functions muss mittels GameManager.OnGameStateChanged += GameManagerOnGameStateChanged;
    // das Abboniert werden
    // Dann dort eine Funktion anleegen: 
    // private void GameManagerOnGameStateChanged(GameState obj){} <-- wird jedes mal ausgeführt wenn der GameState sich ändert
    // unsubscribe ONDestropy mi t- =
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

    [Server]private void HandleSelectCharacter()
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
