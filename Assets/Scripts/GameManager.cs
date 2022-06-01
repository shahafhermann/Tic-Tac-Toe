using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class GameManager : MonoBehaviour 
{
    // Singleton, so I can easily grab it from anywhere in the project.
    public static GameManager instance;

    public GameState gameState;
    public int[,] _tiles = new int[3, 3];
    public static event Action<GameState> OnGameStateChange;

    private int _moveCount = 0;
    private int _winCondition = 0;

    [Header("Token Sprites")] 
    public Sprite xSprite;
    public Sprite oSprite;
    public Sprite emptyToken;
    
    public Sprite background;
    public ButtonEditor buildAssetBundleButton;
    
    private void Awake() 
    {
        instance = this;
    }
    
    private void Start() 
    {
        UpdateGameState(GameState.XTurn);
    }

    void Update()
    {
        
    }

    public int GetMoveCount()
    {
        return _moveCount;
    }

    public void UpdateGameState(GameState newState) 
    {
        gameState = newState;

        switch (newState) 
        {
            case GameState.Menu:
                break;
            case GameState.XTurn:
                _moveCount++;
                break;
            case GameState.OTurn:
                _moveCount++;
                break;
            case GameState.EndGame:
                HandleEndGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    public void EndTurn(int i, int j, int mark)
    {
        _winCondition = CheckWinCondition(i, j, mark);
        if (_winCondition == 0)
            UpdateGameState(gameState == GameState.XTurn ? GameState.OTurn : GameState.XTurn);
        else // GameState not updated - The game ended
            UpdateGameState(GameState.EndGame);
    }
    
    private int CheckWinCondition(int x, int y, int mark)
    {
        // Check row
        for (var i = 0; i < 3; i++)
        {
            if (GameManager.instance._tiles[x, i] != mark) 
                break;
            if (i == 2)
                return 1;
        }
        
        // Check column
        for (var i = 0; i < 3; i++)
        {
            if (GameManager.instance._tiles[i, y] != mark) 
                break;
            if (i == 2)
                return 1;
        }
        
        // Check diagonal
        if (x == y) // We're on the diagonal
        {
            for (var i = 0; i < 3; i++)
            {
                if (GameManager.instance._tiles[i, i] != mark) 
                    break;
                if (i == 2)
                    return 1;
            }
        }
            
        // Check anti-diagonal
        if (x + y == 2) // We're on the anti-diagonal 
        {
            for (var i = 0; i < 3; i++)
            {
                if (GameManager.instance._tiles[i, 2 - i] != mark) 
                    break;
                if (i == 2)
                    return 1;
            }
        }

        // Check draw
        if(GameManager.instance.GetMoveCount() == 9)
            return -1;

        return 0;
    }

    private void HandleEndGame()
    {
        if (_winCondition == 1)
            Debug.Log(_moveCount % 2 == 1 ? "X Won!" : "O Won!");
        else
            Debug.Log("Draw!");
    }
}

public enum GameState 
{
    Menu,
    XTurn,
    OTurn,
    EndTurn,
    EndGame
}
