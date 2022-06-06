using TMPro;
using UnityEngine;
using UnityEngine.UI;

/*
 * Singleton class, used for handling all UI-related logic and mechanics. 
 */
public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    
    private const string WinnerTextMsg = "Player {0} wins!";  // {0} is replaced by the winning player's number
    private const string DrawTextMsg = "Draw";
    
    public GameObject endScreen;
    public GameObject hud;
    public GameObject menu;
    public TextMeshProUGUI winnerText;
    public Button undoButton;
    public GameObject movingPanel;
    public TMP_Dropdown difficultyDropDown;
    public TextMeshProUGUI timerText;
    public SpriteRenderer backgroundObject;
    public Sprite background;
    public Sprite xSprite;
    public Sprite oSprite;
    public Sprite emptyToken;
    public Sprite xTurnSprite;
    public Sprite oTurnSprite;
    public Sprite emptyTurnSprite;
    public Sprite humanRight;
    public Sprite humanLeft;
    public Sprite computerRight;
    public Sprite computerLeft;
    public SpriteRenderer bear;
    public Sprite bearLookLeft;
    public Sprite bearLookRight;
    
    private SpriteRenderer _p1SpriteRenderer;
    private SpriteRenderer _p2SpriteRenderer;
    private SpriteRenderer _p2TurnTile;
    private SpriteRenderer _p1TurnTile;
    
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
            case GameState.NewGame:
                hud.SetActive(true);
                menu.SetActive(false);
                endScreen.SetActive(false);
                break;
            case GameState.EndGame:
                hud.SetActive(false);
                endScreen.SetActive(true);
                break;
            case GameState.Menu:
                backgroundObject.sprite = background;
                hud.SetActive(false);
                menu.SetActive(true);
                endScreen.SetActive(false);
                break;
            case GameState.P1Turn:
                bear.sprite = bearLookRight;
                break;
            case GameState.P2Turn:
                bear.sprite = bearLookLeft;
                break;
        }
    }

    private void Start()
    {
        _p2TurnTile = GameObject.Find("TurnTile_L").GetComponent<SpriteRenderer>();
        _p1TurnTile = GameObject.Find("TurnTile_R").GetComponent<SpriteRenderer>();
        _p1SpriteRenderer = GameObject.Find("Player1").GetComponent<SpriteRenderer>();
        _p2SpriteRenderer = GameObject.Find("Player2").GetComponent<SpriteRenderer>();
        difficultyDropDown.value = 1;
    }

    private void Update()
    {
        // undoButton.interactable = GameManager.Instance.GetMoveCount() > 0;

        if (!GameManager.Instance.TimerRunning()) return;  // If timer isn't running don't do the following.
        
        if (GameManager.Instance.GetTimeRemaining() > 0) {
            GameManager.Instance.UpdateTimeRemaining(Time.deltaTime);
                
            if (GameManager.Instance.timer < 0)
                GameManager.Instance.timer = 0;

            timerText.text = "00:" + Mathf.Ceil(GameManager.Instance.GetTimeRemaining() % 60).ToString("00");
            timerText.color = GameManager.Instance.GetTimeRemaining() < 3 ? 
                new Color(1, 0.168f, 0.219f, 1) : 
                new Color(1, 1, 1, 1);
        }
        else
        {
            GameManager.Instance.TimeOut();
                
        }
    }

    public void SwapTextures(int xPlayer)
    {
        if (xPlayer == 1)  // Player1 plays X
        {
            _p1TurnTile.sprite = GameManager.Instance.gameState == GameState.P1Turn ? xTurnSprite : emptyTurnSprite;
            _p2TurnTile.sprite = GameManager.Instance.gameState == GameState.P1Turn ? emptyTurnSprite : oTurnSprite;
        }
        else  // Player2 plays X
        {
            _p1TurnTile.sprite = GameManager.Instance.gameState == GameState.P1Turn ? oTurnSprite : emptyTurnSprite;
            _p2TurnTile.sprite = GameManager.Instance.gameState == GameState.P1Turn ? emptyTurnSprite : xTurnSprite;
        }
    }

    public void SetPlayers(bool is1Human, bool is2Human)
    {
        _p1SpriteRenderer.sprite = is1Human ? humanRight : computerRight;
        _p2SpriteRenderer.sprite = is2Human ? humanLeft : computerLeft;
    }

    public void SetWinner(bool timeOut = false, int timedOutPlayer = 0, bool draw = false, int xPlayer = 0)
    {
        if (timeOut)
        {
            winnerText.text = timedOutPlayer switch
            {
                1 => string.Format(WinnerTextMsg, 2),
                2 => string.Format(WinnerTextMsg, 1),
                _ => DrawTextMsg
            };
        }
        else
        {
            winnerText.text = draw ? 
                DrawTextMsg : 
                string.Format(WinnerTextMsg, GameManager.Instance.GetMoveCount() % 2 == 1 ? xPlayer : xPlayer % 2 + 1);
        }
    }
}
