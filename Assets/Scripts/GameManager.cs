using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;
using Random = System.Random;

public class GameManager : MonoBehaviour 
{
    // Singleton, so I can easily grab it from anywhere in the project.
    public static GameManager Instance;

    public GameState gameState;
    public Stack Undo = new Stack();
    public int[,] tiles = new int[3, 3];
    public GameObject[,] tileObjects = new GameObject[3, 3];
    public static event Action<GameState> OnGameStateChange;
    private SpriteRenderer _leftTurnTile;
    private SpriteRenderer _rightTurnTile;

    private int _moveCount = 0;
    private int _winCondition = 0;
    private GameState _timeOutWinner;
    private bool _newGame = true;
    private bool _xIsRight = true;
    private bool _timerRunning = false;
    private bool _timeOut = false;
    private float _timeRemaining;
    
    private const string WinnerTextMsg = "Player {0} wins!";  // {0} is replaced by the winning player's number
    private const string DrawTextMsg = "Draw";

    [Header("Setup")] 
    public float timer = 5f;
    public Sprite xSprite;
    public Sprite oSprite;
    public Sprite emptyToken;
    public Sprite xTurnSprite;
    public Sprite oTurnSprite;
    public Sprite emptyTurnSprite;
    public Sprite background;
    public ButtonEditor buildAssetBundleButton;
    


    private void Awake() 
    {
        Instance = this;
        
        SetTileMap();
        _leftTurnTile = GameObject.Find("TurnTile_L").GetComponent<SpriteRenderer>();
        _rightTurnTile = GameObject.Find("TurnTile_R").GetComponent<SpriteRenderer>();
    }
    
    private void Start()
    {
        UpdateGameState(GameState.NewGame);
    }

    public void UpdateGameState(GameState newState) 
    {
        gameState = newState;

        switch (newState) 
        {
            case GameState.Menu:
                break;
            case GameState.NewGame:
                HandleNewGame();
                break;
            case GameState.XTurn:
                HandleXTurn();
                break;
            case GameState.OTurn:
                HandleOTurn();
                break;
            case GameState.EndGame:
                HandleEndGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    private void SetTileMap()
    {
        tileObjects[0, 0] = GameObject.Find("Tile_NW");
        tileObjects[0, 1] = GameObject.Find("Tile_N");
        tileObjects[0, 2] = GameObject.Find("Tile_NE");
        tileObjects[1, 0] = GameObject.Find("Tile_W");
        tileObjects[1, 1] = GameObject.Find("Tile_M");
        tileObjects[1, 2] = GameObject.Find("Tile_E");
        tileObjects[2, 0] = GameObject.Find("Tile_SW");
        tileObjects[2, 1] = GameObject.Find("Tile_S");
        tileObjects[2, 2] = GameObject.Find("Tile_SE");
    }

    public void UpdateMoveCount(int step) { _moveCount += step; }

    public int GetMoveCount() { return _moveCount; }

    public float GetTimeRemaining() { return _timeRemaining; }

    public void UpdateTimeRemaining(float time) { _timeRemaining -= time;}
    
    public void TimeOut()
    {
        _timeOut = true;
        _timeOutWinner = gameState;
        UpdateGameState(GameState.EndGame);
    }

    public bool TimerRunning() { return _timerRunning; }

    private void HandleNewGame()
    {
        _newGame = true;
        _moveCount = 0;
        _timerRunning = true;
        _timeOut = false;
        _timeRemaining = timer;
        UpdateGameState(GameState.XTurn);
    }

    private void HandleOTurn()
    {
        _moveCount++;
        SwapTextures(_xIsRight);
    }

    private void HandleXTurn()
    {
        _moveCount++;

        if (_newGame)  // Randomize which player will play as X
        {
            Random rnd = new Random();
            var randomResult = rnd.Next(1, 3);
            _xIsRight = randomResult == 1;
            _newGame = false;
        }
        
        SwapTextures(_xIsRight);
    }

    private void SwapTextures(bool xSide)
    {
        if (xSide)  // X is played by the player on the right
        {
            _leftTurnTile.sprite = gameState == GameState.XTurn ? emptyTurnSprite : oTurnSprite;
            _rightTurnTile.sprite = gameState == GameState.XTurn ? xTurnSprite : emptyTurnSprite;
        }
        else  // X is played by the player on the left
        {
            _leftTurnTile.sprite = gameState == GameState.XTurn ? xTurnSprite : emptyTurnSprite;
            _rightTurnTile.sprite = gameState == GameState.XTurn ? emptyTurnSprite : oTurnSprite;
        }
    }

    public void EndTurn(bool undo, int i = 0, int j = 0, int mark = 0)
    {
        _timeRemaining = timer;
        _winCondition = CheckWinCondition(i, j, mark);
        if (_winCondition == 0 || undo)
            UpdateGameState(gameState == GameState.XTurn ? GameState.OTurn : GameState.XTurn);
        else // GameState not updated - The game ended
            UpdateGameState(GameState.EndGame);
    }
    
    private int CheckWinCondition(int x, int y, int mark)
    {
        // Check row
        for (var i = 0; i < 3; i++)
        {
            if (tiles[x, i] != mark) 
                break;
            if (i == 2)
                return 1;
        }
        
        // Check column
        for (var i = 0; i < 3; i++)
        {
            if (tiles[i, y] != mark) 
                break;
            if (i == 2)
                return 1;
        }
        
        // Check diagonal
        if (x == y) // We're on the diagonal
        {
            for (var i = 0; i < 3; i++)
            {
                if (tiles[i, i] != mark) 
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
                if (tiles[i, 2 - i] != mark) 
                    break;
                if (i == 2)
                    return 1;
            }
        }

        // Check draw
        if(_moveCount == 9)
            return -1;

        return 0;
    }

    private void HandleEndGame()
    {
        if (_timeOut)
        {
            switch (_timeOutWinner)
            {
                case GameState.XTurn:
                    UIManager.Instance.winnerText.text = string.Format(WinnerTextMsg, "2");
                    break;
                case GameState.OTurn:
                    UIManager.Instance.winnerText.text = string.Format(WinnerTextMsg, "1");
                    break;
                default:
                    UIManager.Instance.winnerText.text = DrawTextMsg;
                    break;
            }
        }
        else
        {
            UIManager.Instance.winnerText.text = _winCondition == 1 ? 
                string.Format(WinnerTextMsg, _moveCount % 2 == 1 ? "1" : "2") : 
                DrawTextMsg;
        }

        _timerRunning = false;
    }
}

public enum GameState 
{
    Menu,
    XTurn,
    OTurn,
    EndGame,
    NewGame,
    TimeOut
}
