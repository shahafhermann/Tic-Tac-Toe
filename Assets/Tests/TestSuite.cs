using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/*
 * A test suite to unit test all required units.
 * Currently only the win condition test is working, due to the fact that it only uses a single Singleton
 * object (GameManager).
 * The rest of the tests require both an instanced class (ButtonController) as well as the GameManager Singleton,
 * which as far as I know isn't possible.
 * I learned this when the project was almost done heavily relies on the Singleton design pattern,
 * and too late for me to change the design.
 */
public class TestSuite
{
    private GameObject _testObject;
    private GameManager _gameManager;
    private ButtonController _buttonController;
    private int[] _tileSetupNotDone;
    private int[] _tileSetup1Win;
    private int[] _tileSetup2Win;
    private int[] _simBoard;
    private Stack moves;

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Scenes/MainScene");

        _testObject = GameObject.Instantiate(new GameObject());
        _buttonController = _testObject.AddComponent<ButtonController>();
        _gameManager = GameManager.Instance;  //_testObject.AddComponent<GameManager>();

        _tileSetupNotDone = new[] { 0, 1, 2, 
                                   0, 1, 2, 
                                   1, 0, 0 };
        _tileSetup1Win = new[] { 2, 1, 2, 
                                0, 1, 2, 
                                1, 1, 0 };
        _tileSetup2Win = new[] { 0, 1, 2, 
                                0, 1, 2, 
                                1, 0, 2 };
        _simBoard = new int[9];
        moves = new Stack();
    }
    
    [UnityTest]
    public IEnumerator TestWinCondition()
    {
        yield return new WaitForSeconds(0.1f);

        // Inject a still going game board
        _gameManager.tiles = _tileSetupNotDone;
        var res = _gameManager.CheckWinCondition(_gameManager.tiles, 1);  // Mark doesn't matter here
        Assert.True(res == 0, "Still playing, expected 0, got " + res);
        
        // Inject a board where p1 wins
        _gameManager.tiles = _tileSetup1Win;
        res = _gameManager.CheckWinCondition(_gameManager.tiles, 1);
        Assert.True(res == 1, "P1 Won, expected 1, got " + res);
        
        // Inject a board where p2 wins
        _gameManager.tiles = _tileSetup2Win;
        res = _gameManager.CheckWinCondition(_gameManager.tiles, 2);
        Assert.True(res == 1, "P2 Won, expected 1, got " + res);
        
        // Check draw. Draw relies on the move count rather than board state, so simulate a game.
        _gameManager.tiles = new int[9];
        ResetMoveCount();
        _gameManager.EndTurn(1);
        _gameManager.EndTurn(0);
        _gameManager.EndTurn(3);
        _gameManager.EndTurn(2);
        _gameManager.EndTurn(4);
        _gameManager.EndTurn(5);
        _gameManager.EndTurn(6);
        _gameManager.EndTurn(7);
        _gameManager.EndTurn(8);
        res = _gameManager.CheckWinCondition(_gameManager.tiles, 1);  // Mark doesn't matter here
        Assert.True(res == -1, "Draw, expected -1, got " + res);
    }

    /*
     * This test is broken since it uses a MonoBehaviour class with a Singleton dependency (GameManager).
     */
    [UnityTest]
    public IEnumerator TestHint()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Reset
        _gameManager.tiles = new int[9];
        ResetMoveCount();

        _buttonController.OnHintPress();
        // Check that the hint index is actually available
        //Assert.True(_gameManager.tiles[_buttonController.hintIndex] == 0, "Hint Index is available at: " + _buttonController.hintIndex);
    }
    
    /*
     * This test is broken since it uses a MonoBehaviour class with a Singleton dependency (GameManager).
     */
    [UnityTest]
    public IEnumerator TestUndo()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Reset
        _gameManager.tiles = new int[9];
        _simBoard = new int[9];
        ResetMoveCount();

        // Check empty board press, should do nothing.
        SimulateMove(undo: true);
        CollectionAssert.AreEqual(_gameManager.tiles, _simBoard, "Board should be empty. Got: " + _gameManager.tiles);
        Assert.True(_gameManager.GetMoveCount() == 0, "Move count should be 0. Got: " + _gameManager.GetMoveCount());
        
        // Make 1 move and undo.
        SimulateMove(move: 1);
        // Debug.Log("MoveCount in test: " + _gameManager.GetMoveCount());
        SimulateMove(undo: true);
        CollectionAssert.AreEqual(_gameManager.tiles, _simBoard, "Board should be empty. Got: " + _gameManager.tiles);
        Assert.True(_gameManager.GetMoveCount() == 0, "Move count should be 0. Got: " + _gameManager.GetMoveCount());
        
        // Again with more moves.
        // _gameManager.EndTurn(move: 1);
        // _gameManager.EndTurn(move: 2);
        // _buttonController.OnUndoPress();
        // CollectionAssert.AreEqual(_gameManager.tiles, _simBoard, "Board should be empty. Got: " + _gameManager.tiles);
        // Assert.True(_gameManager.GetMoveCount() == 0, "Move count should be 0. Got: " + _gameManager.GetMoveCount());
    }

    private void SimulateMove(int move = -1, bool undo = false)
    {
        if (undo)
        {
            /* This won't work since the GameManager instance referenced from this buttonController
             * instance is different then the one I'm using. Since this is a Singleton I have no way
             * of getting the actual used GameManager Instance.
             * I only discovered the Unit Testing feature while working on this project and after most
             * of it was already done. If I had known better before, I wouldn't use Singletons at all.
             * */
            _buttonController.OnUndoPress();
            if (_gameManager.GetMoveCount() > 0)
            {
                _simBoard[(int) moves.Pop()] = 0;
            }
        }
        else
        {
            _simBoard[move] = _gameManager.GetPlayerMark();
            _gameManager.EndTurn(move);
            moves.Push(move);
        }
    }

    private void ResetMoveCount()
    {
        while (_gameManager.GetMoveCount() > 0)
        {
            _gameManager.UpdateMoveCount(-1);
        }
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(_testObject);
    }

}
