using UnityEngine;

public class TileController : MonoBehaviour
{
    private void OnMouseUpAsButton()
    {
        // If it's not a player's turn at all, do nothing.
        if (GameManager.Instance.gameState != GameState.P1Turn && GameManager.Instance.gameState != GameState.P2Turn)
            return;
        
        // If this is the computer's turn, do nothing.
        if ((GameManager.Instance.gameState == GameState.P1Turn && GameManager.Instance.p1 == Player.Computer) ||
            (GameManager.Instance.gameState == GameState.P2Turn && GameManager.Instance.p2 == Player.Computer))
        {
            return;
        }

        var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2D = new Vector2(mousePos.x, mousePos.y);
        
        var hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            GameManager.Instance.PlayerMove(hit.collider);
        }
    }

    public static int GetTileLocation(string tileName)
    {
        switch (tileName)
        {
            case "Tile_NW":
                return 0;
            case "Tile_N":
                return 1;
            case "Tile_NE":
                return 2;
            case "Tile_W":
                return 3;
            case "Tile_M":
                return 4;
            case "Tile_E":
                return 5;
            case "Tile_SW":
                return 6;
            case "Tile_S":
                return 7;
            case "Tile_SE":
                return 8;
            default:
                Debug.Log("While marking the tile, the GameState wasn't X or O.");
                return (-1);
        }
    }

    public static void ClearBoard()
    {
        for (var i = 0; i < 9; i++)
        {
            if (GameManager.Instance.tiles[i] != 0)
            {
                ClearTile(i);
            }
        }
    }
    
    public static void ClearTile(int i)
    {
        GameManager.Instance.tileObjects[i].sprite = UIManager.Instance.emptyToken;
        GameManager.Instance.tiles[i] = 0;
    }
}

