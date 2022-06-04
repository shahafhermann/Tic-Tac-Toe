using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

public class TestSuite
{

    // // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
    // // `yield return null;` to skip a frame.
    // [UnityTest]
    // public IEnumerator NewTestScriptWithEnumeratorPasses()
    // {
    //     // Use the Assert class to test conditions.
    //     // Use yield to skip a frame.
    //     yield return null;
    // }

    private GameObject _testObject;
    private GameManager _gameManager;
    private ButtonController _buttonController;
    private int[] _tileSetupNotDone;
    private int[] _tileSetup1Win;
    private int[] _tileSetup2Win;

    [SetUp]
    public void Setup()
    {
        SceneManager.LoadScene("Scenes/MainScene");
        
        _testObject = GameObject.Instantiate(new GameObject());
        _gameManager = _testObject.AddComponent<GameManager>();
        _buttonController = _testObject.AddComponent<ButtonController>();

        _tileSetupNotDone = new[] { 0, 1, 2, 
                                   0, 1, 2, 
                                   1, 0, 0 };
        _tileSetup1Win = new[] { 2, 1, 2, 
                                0, 1, 2, 
                                1, 1, 0 };
        _tileSetup2Win = new[] { 0, 1, 2, 
                                0, 1, 2, 
                                1, 0, 2 };
    }
    
    [UnityTest]
    public IEnumerator TestWinCondition()
    {
        yield return new WaitForSeconds(0.1f);

        // Inject a still going game board
        _gameManager.tiles = _tileSetupNotDone;
        var res = _gameManager.CheckWinCondition(1);  // Mark doesn't matter here
        Assert.True(res == 0, "Still playing, expected 0, got " + res);
        
        // Inject a board where p1 wins
        _gameManager.tiles = _tileSetup1Win;
        res = _gameManager.CheckWinCondition(1);
        Assert.True(res == 1, "P1 Won, expected 1, got " + res);
        
        // Inject a board where p2 wins
        _gameManager.tiles = _tileSetup2Win;
        res = _gameManager.CheckWinCondition(2);
        Assert.True(res == 1, "P2 Won, expected 1, got " + res);
        
        // Check draw. Draw relies on the move count rather than board state, so simulate a game.
        while (_gameManager.GetMoveCount() > 0)
        {
            _gameManager.UpdateMoveCount(-1);
        }
        _gameManager.EndTurn(move: 1);
        _gameManager.EndTurn(move: 0);
        _gameManager.EndTurn(move: 3);
        _gameManager.EndTurn(move: 2);
        _gameManager.EndTurn(move: 4);
        _gameManager.EndTurn(move: 5);
        _gameManager.EndTurn(move: 6);
        _gameManager.EndTurn(move: 7);
        _gameManager.EndTurn(move: 8);
        res = _gameManager.CheckWinCondition(1);  // Mark doesn't matter here
        Assert.True(res == -1, "Draw, expected -1, got " + res);
    }

    [UnityTest]
    public IEnumerator TestUndo()
    {
        yield return new WaitForSeconds(0.1f);
        
        // Reset the game
        while (_gameManager.GetMoveCount() > 0)
        {
            _gameManager.UpdateMoveCount(-1);
        }
        _gameManager.tiles = new int[9];

        // Create a board simulation and simulate moves, then undo and check the board again.
        var simBoard = new int[9];

        // Check empty board press, should do nothing.
        _buttonController.OnUndoPress();
        CollectionAssert.AreEqual(_gameManager.tiles, simBoard, "Board should be empty. Got: " + _gameManager.tiles);
        Assert.True(_gameManager.GetMoveCount() == 0, "Move count should be 0. Got: " + _gameManager.GetMoveCount());
        
        // Make 1 move and undo.
        _gameManager.EndTurn(move: 1);
        _buttonController.OnUndoPress();
        CollectionAssert.AreEqual(_gameManager.tiles, simBoard, "Board should be empty. Got: " + _gameManager.tiles);
        Assert.True(_gameManager.GetMoveCount() == 0, "Move count should be 0. Got: " + _gameManager.GetMoveCount());
    }

    [TearDown]
    public void TearDown()
    {
        GameObject.Destroy(_testObject);
    }

}
