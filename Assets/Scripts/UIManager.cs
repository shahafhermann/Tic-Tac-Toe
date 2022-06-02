using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    public GameObject endScreen;
    public GameObject hud;
    public GameObject menu;
    public TextMeshProUGUI winnerText;
    public Button undoButton;
    
    private void Awake()
    {
        Instance = this;
        
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
        switch (state)
        {
            // _hud.SetActive(state == GameState.OTurn || state == GameState.XTurn);
            case GameState.NewGame:
                hud.SetActive(true);
                // menu.SetActive(false);
                endScreen.SetActive(false);
                break;
            case GameState.EndGame:
                hud.SetActive(false);
                endScreen.SetActive(true);
                break;
            case GameState.Menu:
                hud.SetActive(false);
                // menu.SetActive(true);
                break;
        }
    }

    void Update()
    {
        undoButton.interactable = GameManager.Instance.GetMoveCount() > 1;
    }
}
