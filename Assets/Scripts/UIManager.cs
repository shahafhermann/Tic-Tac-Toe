using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private GameObject _hud;
    public Animator animator;
    
    private void Awake()
    {
        // Subscribe to the gameState change event.
        GameManager.OnGameStateChange += OnGameStateChanged;
    }

    private void OnDestroy()
    {
        // Unsubscribe to the gameState change event. Do it on destroy to prevent memory leaks.
        GameManager.OnGameStateChange -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        // _hud.SetActive(state == GameState.OTurn || state == GameState.XTurn);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
                    GameManager.Instance.tileObjects[i, j].GetComponent<SpriteRenderer>().sprite = GameManager.Instance.emptyToken;
                }
            }
        }
        
        GameManager.Instance.UpdateGameState(GameState.NewGame);
    }
}
