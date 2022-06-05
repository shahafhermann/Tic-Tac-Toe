using System;
using System.Collections;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

/*
 * A class defining all button functionalities within the game.
 */
public class ButtonController : MonoBehaviour
{
    private Animator _hintAnimator;
    private Animator _settingsAnimator;

    private void Start()
    {
        _settingsAnimator = UIManager.Instance.movingPanel.GetComponent<Animator>();
    }

    public void OnSettingsPress()
    {
        _settingsAnimator.SetBool("OpenSettings", true);
    }
    
    public void OnReskinPress(TMP_InputField inputPath)
    {
        var assetBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, inputPath.text));
        var assets = assetBundle.LoadAllAssets();
        UIManager.Instance.xSprite = (Sprite) assets[5];
        UIManager.Instance.oSprite = (Sprite) assets[3];
        UIManager.Instance.background = (Sprite) assets[1];
    }
    
    public void OnPlayModeDropDown(TMP_Dropdown dropDown)
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
    
    public void OnDifficultyDropDown(TMP_Dropdown dropDown)
    {
        GameManager.Instance.hardMode = dropDown.value switch
        {
            0 => // Dumb
                false,
            1 => // Genius
                true,
            _ => GameManager.Instance.hardMode  // Default
        };
    }

    public void OnBackPress(bool fromSettings = false)
    {
        if (fromSettings)
        {
            _settingsAnimator.SetBool("OpenSettings", false);
        }
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
        _hintAnimator = GameManager.Instance.tileObjects[availableMoves[hintIndex]].GetComponent<Animator>();
        StartCoroutine(WaitForHint());
    }

    private IEnumerator WaitForHint()
    {
        _hintAnimator.SetBool("Hint", true);
        yield return new WaitForSeconds(2);
        _hintAnimator.SetBool("Hint", false);
    }

    public void OnRestartPress()
    {
        TileController.ClearBoard();
        GameManager.Instance.UpdateGameState(GameState.NewGame);
    }
}