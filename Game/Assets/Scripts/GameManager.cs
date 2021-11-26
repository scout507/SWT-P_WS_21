using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    void Awake(){
        Instance = this;
    }

    void Start(){
        UpdateGameState(GameState.SelectCharacter);
    }

    public void UpdateGameState(GameState newState){
        State = newState;

        switch (newState){
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

    private HandleSelectCharacter(){

    }
 
}

public enum GameState{
        SelectCharacter,
        Phase1,
        Phase2,
        Phase3,
        Phase4,
        Phase5,
        Victory,
        Lose
    }
