using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using Random = System.Random;

/*
 * Singleton class, used for handling all game-related logic and mechanics. 
 */
public class GameManager : MonoBehaviour 
{
    // Singleton, so I can easily grab it from anywhere in the project.
    public static GameManager Instance;

    public GameState gameState;
    public Player p1;
    public Player p2;
    public Stack Undo = new Stack();
    public int[] tiles = new int[9];
    public SpriteRenderer[] tileObjects;
    public bool hardMode = true;
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

    /*
     * Handle all game state updates.
     */
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

    /*
     * Handle a new game state. Reset all parameters, randomize the X player and let it start.
     */
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

    /*
     * Handle a new turn
     */
    private void HandleNewTurn()
    {
        UIManager.Instance.SwapTextures(_xPlayer);
        
        // Decide if the computer should play now
        if ((gameState == GameState.P1Turn && p1 == Player.Computer) ||
            (gameState == GameState.P2Turn && p2 == Player.Computer))
        {
            StartCoroutine(AIMove());
        }
    }
    
    private IEnumerator AIMove()
    {
        yield return new WaitForSeconds(aiMoveWaitTime);  // Make the AI wait before playing

        var selectedMove = -1;
        
        if (hardMode)
        {
            selectedMove = BestMove();
        }
        if (selectedMove == -1)  // If it's not hard mode or if BestMove() returned -1.
        {
            selectedMove = RandomMove();
        }
        
        tileObjects[selectedMove].sprite = GetPlayerSprite();
        EndTurn(move: selectedMove);
    }

    private int RandomMove()
    {
        var availableMoves = Enumerable.Range(0, tiles.Length).Where(i => tiles[i] == 0).ToArray();
        var rnd = new Random();
        var randomIndex = rnd.Next(0, availableMoves.Length);
        return availableMoves[randomIndex];
    }

    private int BestMove() 
    {
        // AI to make its turn
        var bestScore = int.MinValue;
        var move = -1;
        for (var i = 0; i < 9; i++) 
        {
            if (tiles[i] == 0) {  // Is the spot available?
                tiles[i] = GetPlayerMark();
                var newBoard = new int[9];
                tiles.CopyTo(newBoard, 0);
                var score = Minimax(newBoard, 0, false);
                tiles[i] = 0;
                if (score > bestScore) 
                {
                    bestScore = score;
                    move = i;
                }
            }
        }
        return move;
    }
    
    private int Minimax(int[] board, int depth, bool isMaximizing) 
    {
        // Base Cases - Board terminal state
        if (CheckWinCondition(board, GetPlayerMark()) == GetPlayerMark()) // The computer wins in this state
        {
            return 10 - depth;
        }
        if (CheckWinCondition(board, GetPlayerMark() % 2 + 1) == GetPlayerMark() % 2 + 1) // // The computer doesn't win in this state
        {
            return depth - 10;
        }
        if (CheckWinCondition(board, GetPlayerMark()) == -1) // draw
        {
            return 0;
        }
    
        if (isMaximizing)
        {  // Computer's move, maximize score
            var bestScore = int.MinValue;
            for (var i = 0; i < 9; i++)  // Try all moves
            {
                if (board[i] == 0)  // Is the move valid?
                {
                    // board[i] = GetPlayerMark();
                    var newBoard = new int[9];
                    board.CopyTo(newBoard, 0);  // Create a deep copy of the board to send in to the recursion
                    newBoard[i] = GetPlayerMark();
                    var score = Minimax(newBoard, depth + 1, false);
                    // board[i] = 0;
                    bestScore = Math.Max(score, bestScore);
                }
            }
            return bestScore;
        } 
        else 
        {  // Player's move, minimize score
            var bestScore = int.MaxValue;
            for (var i = 0; i < 9; i++)    // Try all moves
            {
                if (board[i] == 0)   // Is the move valid?
                {
                    // board[i] = GetPlayerMark() % 2 + 1;
                    var newBoard = new int[9];
                    board.CopyTo(newBoard, 0);  // Create a deep copy of the board to send in to the recursion
                    newBoard[i] = GetPlayerMark() % 2 + 1;
                    var score = Minimax(newBoard, depth + 1, true);
                    // board[i] = 0;
                    bestScore = Math.Min(score, bestScore);
                }
            }
            return bestScore;
        }
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
            _winCondition = CheckWinCondition(tiles, GetPlayerMark());
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
    public int CheckWinCondition(int[] board, int mark)
    {
        // All possibilities for a win
        if ((board[0] == mark && board[1] == mark && board[2] == mark) ||
            (board[3] == mark && board[4] == mark && board[5] == mark) ||
            (board[6] == mark && board[7] == mark && board[8] == mark) ||
            (board[0] == mark && board[3] == mark && board[6] == mark) ||
            (board[1] == mark && board[4] == mark && board[7] == mark) ||
            (board[2] == mark && board[5] == mark && board[8] == mark) ||
            (board[0] == mark && board[4] == mark && board[8] == mark) ||
            (board[2] == mark && board[4] == mark && board[6] == mark)) 
        {
            return mark;  // Return the winner's number
        }

        // Check draw
        for (var i = 0; i < 9; i++)
        {
            if (board[i] == 0)
            {
                return 0;
            }
        }
        
        return -1;  // Draw
    }

    private void HandleEndGame()
    {
        if (_timeOut)
        {
            UIManager.Instance.SetWinner(timeOut: true, timedOutPlayer:_timedOutPlayer);
        }
        else
        {
            UIManager.Instance.SetWinner(draw: _winCondition == -1, xPlayer: _xPlayer);
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
        return gameState == GameState.P1Turn ? 1 : 2;
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
