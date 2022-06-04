using System.Collections;
using System.Linq;
using UnityEngine;
using Random = System.Random;

public class ButtonController : MonoBehaviour
{
    public Animator animator;

    public void OnPvpPress()
    {
        GameManager.Instance.p1 = Player.Human;
        GameManager.Instance.p2 = Player.Human;
        UIManager.Instance.SetPlayers(true, true);
        OnRestartPress();
    }
    
    public void OnPvcPress()
    {
        GameManager.Instance.p1 = Player.Human;
        GameManager.Instance.p2 = Player.Computer;
        UIManager.Instance.SetPlayers(true, false);
        OnRestartPress();
    }
    
    public void OnCvcPress()
    {
        GameManager.Instance.p1 = Player.Computer;
        GameManager.Instance.p2 = Player.Computer;
        UIManager.Instance.SetPlayers(false, false);
        OnRestartPress();
    }

    public void OnBackPress()
    {
        GameManager.Instance.UpdateGameState(GameState.Menu);
    }
    
    public void OnUndoPress()
    {
        if (GameManager.Instance.Undo.Count > 0)  // Sanity check
        {
            GameManager.Instance.EndTurn(undo: true);
        }
    }

    public void OnHintPress()
    {
        // Give a random tile as hint
        var availableMoves = Enumerable.Range(0, GameManager.Instance.tiles.Length).Where(i => GameManager.Instance.tiles[i] == 0).ToArray();
        var rnd = new Random();
        var randomIndex = rnd.Next(0, availableMoves.Length);
        animator = GameManager.Instance.tileObjects[availableMoves[randomIndex]].GetComponent<Animator>();
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
