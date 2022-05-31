using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private SpriteRenderer[,] tiles = new SpriteRenderer[3, 3];

    void Awake()
    {
        tiles[0, 0] = GameObject.Find("Tile_NW").GetComponent<SpriteRenderer>();
        tiles[0, 1] = GameObject.Find("Tile_N").GetComponent<SpriteRenderer>();
        tiles[0, 2] = GameObject.Find("Tile_NE").GetComponent<SpriteRenderer>();
        tiles[1, 0] = GameObject.Find("Tile_W").GetComponent<SpriteRenderer>();
        tiles[1, 1] = GameObject.Find("Tile_M").GetComponent<SpriteRenderer>();
        tiles[1, 2] = GameObject.Find("Tile_E").GetComponent<SpriteRenderer>();
        tiles[2, 0] = GameObject.Find("Tile_SW").GetComponent<SpriteRenderer>();
        tiles[2, 1] = GameObject.Find("Tile_S").GetComponent<SpriteRenderer>();
        tiles[2, 2] = GameObject.Find("Tile_SE").GetComponent<SpriteRenderer>();
    }

    private void OnMouseUpAsButton()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            
        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider != null)
        {
            GameManager.instance.MarkTile(hit.collider);
        }
    }
}
