using System;
using System.Collections;
using System.Linq;
// using UnityEditor.UI;
using UnityEngine;
using Random = System.Random;

public class GameManager : MonoBehaviour 
{
    // Singleton, so I can easily grab it from anywhere in the project.
    public static GameManager Instance;
    public ButtonController bc;

    public GameState gameState;
    public Player p1;
    public Player p2;
    public Stack Undo = new Stack();
    public int[] tiles = new int[9];
    public SpriteRenderer[] tileObjects;
    public static event Action<GameState> OnGameStateChange;

    private int _moveCount;
    private int _winCondition;
    private int _timedOutPlayer;
    private int _xPlayer;
    private bool _timerRunning;
    private bool _timeOut;
    private float _timeRemaining;

    [Header("Setup")] 
    public float timer = 5f;
    public float aiMoveWaitTime = 0.6f;
    public Sprite background;
    // public ButtonEditor buildAssetBundleButton;

    private void Awake() 
    {
        Instance = this;
    }
    
    private void Start()
    {
        UpdateGameState(GameState.Menu);
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
            case GameState.P1Turn:
                HandleNewTurn();
                break;
            case GameState.P2Turn:
                HandleNewTurn();
                break;
            case GameState.EndGame:
                HandleEndGame();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    public void UpdateMoveCount(int step) { _moveCount += step; }

    public int GetMoveCount() { return _moveCount; }

    public float GetTimeRemaining() { return _timeRemaining; }

    public void UpdateTimeRemaining(float time) { _timeRemaining -= time;}
    
    public void TimeOut()
    {
        _timeOut = true;
        _timedOutPlayer = gameState == GameState.P1Turn ? 1 : 2;
        UpdateGameState(GameState.EndGame);
    }

    public bool TimerRunning() { return _timerRunning; }

    private void HandleNewGame()
    {
        _moveCount = 0;
        _timerRunning = true;
        _timeOut = false;
        _timeRemaining = timer;
        
        // Randomize which player will play as X
        Random rnd = new Random();
        var randomResult = rnd.Next(1, 3);
        _xPlayer = randomResult;

        UpdateGameState(_xPlayer == 1 ? GameState.P1Turn : GameState.P2Turn);
    }

    private void HandleNewTurn()
    {
        UIManager.Instance.SwapTextures(_xPlayer);
        
        if ((gameState == GameState.P1Turn && p1 == Player.Computer) ||
            (gameState == GameState.P2Turn && p2 == Player.Computer))
        {
            StartCoroutine(AIMove());
        }
    }
    
    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(aiMoveWaitTime);  // Make the AI wait before playing
        
        var availableMoves = Enumerable.Range(0, tiles.Length).Where(i => tiles[i] == 0).ToArray();
        var rnd = new Random();
        var randomIndex = rnd.Next(0, availableMoves.Length);
        var selectedMove = availableMoves[randomIndex];
        
        tileObjects[selectedMove].sprite = GetPlayerSprite();
        EndTurn(move: selectedMove);
    }

    public void PlayerMove(Collider2D collider)
    {
        var move = TileController.GetTileLocation(collider.name);  // get tile's location in matrix
        if (tiles[move] != 0) return;  // Already marked, do nothing

        collider.GetComponent<SpriteRenderer>().sprite = GetPlayerSprite();
        EndTurn(move: move);
    }

    public void EndTurn(int move = -1, bool undo = false)
    {
        _timeRemaining = timer;
        if (undo)
        {
            _moveCount--;
            var lastMove = (int) Undo.Pop();
            TileController.ClearTile(lastMove);
            UpdateGameState(gameState == GameState.P1Turn ? GameState.P2Turn : GameState.P1Turn);
        }
        else
        {
            _moveCount++;
            tiles[move] = GetPlayerMark();
            Undo.Push(move);
            _winCondition = CheckWinCondition(GetPlayerMark());
            if (_winCondition == 0)
                UpdateGameState(gameState == GameState.P1Turn ? GameState.P2Turn : GameState.P1Turn);
            else // GameState not updated - The game ended
                UpdateGameState(GameState.EndGame);
        }
    }
    
    /**
     * Check if any player won.
     * Return 1 if a player won, -1 if it's a draw or 0 otherwise (Game hasn't ended)
     */
    public int CheckWinCondition(int mark)
    {
        // All possibilities for a win
        if ((tiles[0] == mark && tiles[1] == mark && tiles[2] == mark) ||
            (tiles[3] == mark && tiles[4] == mark && tiles[5] == mark) ||
            (tiles[6] == mark && tiles[7] == mark && tiles[8] == mark) ||
            (tiles[0] == mark && tiles[3] == mark && tiles[6] == mark) ||
            (tiles[1] == mark && tiles[4] == mark && tiles[7] == mark) ||
            (tiles[2] == mark && tiles[5] == mark && tiles[8] == mark) ||
            (tiles[0] == mark && tiles[4] == mark && tiles[8] == mark) ||
            (tiles[2] == mark && tiles[4] == mark && tiles[6] == mark)) 
        {
            return 1;
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
            UIManager.Instance.SetWinner(timeOut: true, timedOutPlayer:_timedOutPlayer);
        }
        else
        {
            UIManager.Instance.SetWinner(draw: _winCondition != 1, xPlayer: _xPlayer);
        }

        _timerRunning = false;
    }
    
    private Sprite GetPlayerSprite()
    {
        var sprite = gameState switch
        {
            GameState.P1Turn => _xPlayer == 1 ? UIManager.Instance.xSprite : UIManager.Instance.oSprite,
            GameState.P2Turn => _xPlayer == 2 ? UIManager.Instance.xSprite : UIManager.Instance.oSprite,
            _ => null
        };

        return sprite;
    }
    
    public int GetPlayerMark()
    {
        return gameState == GameState.P1Turn ? 1 : -1;
    }
}

public enum GameState 
{
    Menu,
    P1Turn,
    P2Turn,
    EndGame,
    NewGame
}

public enum Player
{
    Human,
    Computer
}
