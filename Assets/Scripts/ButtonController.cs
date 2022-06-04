using System;
using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using Random = System.Random;

public class ButtonController : MonoBehaviour
{
    public Animator animator;

    public void OnDropDown(TMP_Dropdown dropDown)
    {
        switch (dropDown.value)
        {
            case 0:
                GameManager.Instance.p1 = Player.Human;
                GameManager.Instance.p2 = Player.Human;
                UIManager.Instance.SetPlayers(true, true);
                break;
            case 1:
                GameManager.Instance.p1 = Player.Human;
                GameManager.Instance.p2 = Player.Computer;
                UIManager.Instance.SetPlayers(true, false);
                break;
            case 2:
                GameManager.Instance.p1 = Player.Computer;
                GameManager.Instance.p2 = Player.Computer;
                UIManager.Instance.SetPlayers(false, false);
                break;
        }
    }

    public void OnBackPress()
    {
        GameManager.Instance.UpdateGameState(GameState.Menu);
    }
    
    /*
     * The instance attribute is used for testing since Unit Tests don't work well with singletons.
     */
    public void OnUndoPress()
    {
        if (GameManager.Instance.GetMoveCount() > 0)  // Sanity check
        {
            GameManager.Instance.EndTurn(undo: true);
        }
    }

    public void OnHintPress()
    {
        // Give a random tile as hint
        var availableMoves = Enumerable.Range(0, GameManager.Instance.tiles.Length).Where(i => GameManager.Instance.tiles[i] == 0).ToArray();
        var rnd = new Random();
        var hintIndex = rnd.Next(0, availableMoves.Length);
        animator = GameManager.Instance.tileObjects[availableMoves[hintIndex]].GetComponent<Animator>();
        StartCoroutine(WaitForHint());
    }

    private IEnumerator WaitForHint()
    {
        animator.SetBool("Hint", true);
        yield return new WaitForSeconds(2);
        animator.SetBool("Hint", false);
    }

    public void OnRestartPress()
    {
        TileController.ClearBoard();
        GameManager.Instance.UpdateGameState(GameState.NewGame);
    }
}
