using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public Animator animator;
    
    public void OnUndoPress()
    {
        var (i, j) = (Tuple<int, int>) GameManager.Instance.Undo.Pop();
        TileController.ClearTile(i, j);
        GameManager.Instance.UpdateMoveCount(-1);
        GameManager.Instance.EndTurn(true);
    }

    public void OnHintPress()
    {
        // Search for the first empty tile, give it as a hint.
        var exit = false;
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (GameManager.Instance.tiles[i, j] == 0)
                {
                    animator = GameManager.Instance.tileObjects[i, j].GetComponent<Animator>();
                    animator.SetBool("Hint", true);
                    StartCoroutine(WaitForHint());
                    exit = true;
                }
                if (exit) break;
            }
            if (exit) break;
        }
    }

    private IEnumerator WaitForHint()
    {
        yield return new WaitForSeconds(2);
        animator.SetBool("Hint", false);
    }

    public void OnRestartPress()
    {
        for (var i = 0; i < 3; i++)
        {
            for (var j = 0; j < 3; j++)
            {
                if (GameManager.Instance.tiles[i, j] != 0)
                {
                    TileController.ClearTile(i, j);
                }
            }
        }
        
        GameManager.Instance.UpdateGameState(GameState.NewGame);
    }
}
