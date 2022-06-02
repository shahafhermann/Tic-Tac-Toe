using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    void Awake()
    {
        // tiles[0, 0] = GameObject.Find("Tile_NW").GetComponent<SpriteRenderer>();
        // tiles[0, 1] = GameObject.Find("Tile_N").GetComponent<SpriteRenderer>();
        // tiles[0, 2] = GameObject.Find("Tile_NE").GetComponent<SpriteRenderer>();
        // tiles[1, 0] = GameObject.Find("Tile_W").GetComponent<SpriteRenderer>();
        // tiles[1, 1] = GameObject.Find("Tile_M").GetComponent<SpriteRenderer>();
        // tiles[1, 2] = GameObject.Find("Tile_E").GetComponent<SpriteRenderer>();
        // tiles[2, 0] = GameObject.Find("Tile_SW").GetComponent<SpriteRenderer>();
        // tiles[2, 1] = GameObject.Find("Tile_S").GetComponent<SpriteRenderer>();
        // tiles[2, 2] = GameObject.Find("Tile_SE").GetComponent<SpriteRenderer>();
    }

    private void OnMouseUpAsButton()
    {
        if (GameManager.Instance.gameState != GameState.EndGame)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            if (hit.collider != null)
            {
                MarkTile(hit.collider);
            }
        }
    }
    
    private void MarkTile(Collider2D tileCollider)
    {
        var (i, j) = GetTileLocation(tileCollider.name);  // get tile's matrix location
        if (GameManager.Instance.tiles[i, j] != 0) return;  // Already marked, do nothing

        var gameState = GameManager.Instance.gameState;
        Sprite sprite;
        int mark;
        switch (gameState)
        {
            case GameState.XTurn:
                sprite = GameManager.Instance.xSprite;
                mark = 1;
                break;
            case GameState.OTurn:
                sprite = GameManager.Instance.oSprite;
                mark = -1;
                break;
            default:
                sprite = GameManager.Instance.emptyToken;
                mark = 0;
                Debug.Log("While marking the tile, the GameState wasn't X or O.");
                break;
        }

        tileCollider.GetComponent<SpriteRenderer>().sprite = sprite;
        GameManager.Instance.tiles[i, j] = mark;
        GameManager.Instance.EndTurn(i, j, mark);
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
}