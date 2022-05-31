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
    public static event Action<GameState> OnGameStateChange;
    
    private int[,] _tiles = new int[3, 3];
    private int moveCount = 0;

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
    
    // Start is called before the first frame update
    private void Start() 
    {
        UpdateGameState(GameState.XTurn);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateGameState(GameState newState) 
    {
        gameState = newState;

        switch (newState) 
        {
            case GameState.Menu:
                break;
            case GameState.XTurn:
                break;
            case GameState.OTurn:
                break;
            case GameState.EndGame:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    public void MarkTile(Collider2D tileCollider)
    {
        moveCount++;
        
        var (i, j) = GetTileLocation(tileCollider.name); // get tile's matrix location
        if (_tiles[i, j] != 0) return; // Already marked, do nothing
        
        switch (gameState)
        {
            case GameState.XTurn:
                tileCollider.GetComponent<SpriteRenderer>().sprite = xSprite;
                _tiles[i, j] = 1;
                CheckWinCondition(i, j, 1);
                UpdateGameState(GameState.OTurn);
                break;
            case GameState.OTurn:
                tileCollider.GetComponent<SpriteRenderer>().sprite = oSprite;
                _tiles[i, j] = -1;
                CheckWinCondition(i, j, -1);
                UpdateGameState(GameState.XTurn);
                break;
            default:
                Debug.Log("While marking the tile, the GameState wasn't X or O.");
                break;
        }
    }

    private static (int i, int j) GetTileLocation(string tileName)
    {
        switch (tileName)
        {
            case "Tile_NW":
                return (0, 0);
            case "Tile_N":
                return (0, 1);
            case "Tile_NE":
                return (0, 2);
            case "Tile_W":
                return (1, 0);
            case "Tile_M":
                return (1, 1);
            case "Tile_E":
                return (1, 2);
            case "Tile_SW":
                return (2, 0);
            case "Tile_S":
                return (2, 1);
            case "Tile_SE":
                return (2, 2);
            default:
                Debug.Log("While marking the tile, the GameState wasn't X or O.");
                return (-1, -1);
        }
    }

    private void CheckWinCondition(int x, int y, int mark)
    {
        // Check column
        for (var i = 0; i < 3; i++)
        {
            if (_tiles[x, i] != mark) 
                break;
            if (i == 2) 
                Debug.Log(mark + " Wins!"); // Win for mark
        }
        
        // Check row
        for (var i = 0; i < 3; i++)
        {
            if (_tiles[i, y] != mark) 
                break;
            if (i == 2) 
                Debug.Log(mark + " Wins!"); // Win for mark
        }
        
        // Check diagonal
        if (x == y) // We're on the diagonal
        {
            for (var i = 0; i < 3; i++)
            {
                if (_tiles[i, i] != mark) 
                    break;
                if (i == 2) 
                    Debug.Log(mark + " Wins!"); // Win for mark
            }
        }
            
        // Check anti-diagonal
        if (x + y == 2) // We're on the anti-diagonal 
        {
            for (var i = 0; i < 3; i++)
            {
                if (_tiles[i, 2 - i] != mark) 
                    break;
                if (i == 2) 
                    Debug.Log(mark + " Wins!"); // Win for mark
            }
        }

        // Check draw
        if(moveCount == 9) 
            Debug.Log("Draw!"); // draw
    }
}

public enum GameState 
{
    Menu,
    XTurn,
    OTurn,
    EndGame
}
