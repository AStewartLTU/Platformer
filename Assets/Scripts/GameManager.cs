
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private enum GameState
    {
        Menu,
        Playing,
        Paused,
        Victory,
        GameOver
    }

    private GameState currentState = GameState.Menu;

    public bool IsPlaying => currentState == GameState.Playing;

    private int score;

    private PlayerMovement player;
    private PlayerHealth playerHealth;

    private Enemy[] enemies;

    private List<GameObject> coins = new List<GameObject>();
    private List<Vector3> coinPositions = new List<Vector3>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        Time.timeScale = 1f;

        player = FindObjectOfType<PlayerMovement>();
        playerHealth = FindObjectOfType<PlayerHealth>();

        enemies = FindObjectsOfType<Enemy>();

        // Find coins in the scene
        Transform[] allObjects = FindObjectsOfType<Transform>();

        foreach (Transform item in allObjects)
        {
            if (item.name.StartsWith("Coin"))
            {
                coins.Add(item.gameObject);
                coinPositions.Add(item.position);
            }
        }
    }

    void Start()
    {
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (currentState == GameState.Playing &&
            Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    // Add points to score
    public void AddScore(int points)
    {
        score += points;
    }

    // Pause game
    public void PauseGame()
    {
        if (currentState != GameState.Playing)
            return;

        SetState(GameState.Paused);
    }

    // Resume game
    public void ResumeGame()
    {
        if (currentState == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }

    // Victory screen
    public void Victory()
    {
        SetState(GameState.Victory);
    }

    // Game Over screen
    public void GameOver()
    {
        SetState(GameState.GameOver);
    }

    // Change game state
    void SetState(GameState newState)
    {
        currentState = newState;

        Time.timeScale =
            newState == GameState.Paused ? 0f : 1f;
    }

    // Start or restart game
    public void StartGame()
    {
        Time.timeScale = 1f;

        score = 0;

        if (player != null)
        {
            player.ResetPlayerPosition();
        }

        if (playerHealth != null)
        {
            playerHealth.ResetHealth();
        }

        // Reset enemies
        foreach (Enemy enemy in enemies)
        {
            if (enemy != null)
            {
                enemy.ResetEnemy();
            }
        }

        // Reset coins
        for (int i = 0; i < coins.Count; i++)
        {
            if (coins[i] != null)
            {
                coins[i].transform.position = coinPositions[i];
                coins[i].SetActive(true);
            }
        }

        SetState(GameState.Playing);
    }

    // Main menu
    public void MainMenu()
    {
        SetState(GameState.Menu);
    }

    // UI and menus
    void OnGUI()
    {
        GUI.skin.label.fontSize = 24;
        GUI.skin.button.fontSize = 24;

        // Gameplay UI
        if (currentState == GameState.Playing)
        {
            int health = playerHealth != null
                ? playerHealth.GetHealth()
                : 0;

            GUI.color = new Color(.05f, .08f, .13f, .82f);

            GUI.Box(
                new Rect(14, 12, 310, 48),
                GUIContent.none
            );

            GUI.color = Color.white;

            GUI.Label(
                new Rect(28, 18, 290, 36),
                "Health: " +
                new string('?', Mathf.Max(0, health)) +
                "     Score: " + score
            );

            GUI.Label(
                new Rect(14, Screen.height - 42, 480, 34),
                "Move: A/D or arrows     Jump: Space     Pause: Esc"
            );

            return;
        }

        // Menu background
        GUI.color = new Color(.04f, .07f, .14f, .94f);

        GUI.Box(
            new Rect(0, 0, Screen.width, Screen.height),
            GUIContent.none
        );

        GUI.color = Color.white;

        float centerX = Screen.width * 0.5f;

        string title =
            currentState == GameState.Menu ? "PLATFORM ADVENTURE" :
            currentState == GameState.Paused ? "PAUSED" :
            currentState == GameState.Victory ? "VICTORY!" :
            "GAME OVER";

        GUI.Label(
            new Rect(centerX - 280, Screen.height * .25f, 560, 70),
            title,
            CenterStyle(42)
        );

        // Main Menu
        if (currentState == GameState.Menu)
        {
            GUI.Label(
                new Rect(centerX - 300, Screen.height * .40f, 600, 45),
                "Reach the golden flag. Stomp enemies and collect coins!",
                CenterStyle(20)
            );

            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .55f, 220, 58),
                "PLAY"))
            {
                StartGame();
            }

            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .55f + 74, 220, 52),
                "EXIT"))
            {
                Application.Quit();
            }
        }

        // Pause Menu
        else if (currentState == GameState.Paused)
        {
            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .48f, 220, 58),
                "RESUME"))
            {
                ResumeGame();
            }

            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .48f + 74, 220, 52),
                "MAIN MENU"))
            {
                MainMenu();
            }
        }

        // Victory and Game Over
        else
        {
            GUI.Label(
                new Rect(centerX - 220, Screen.height * .40f, 440, 46),
                "Final score: " + score,
                CenterStyle(24)
            );

            string buttonText =
                currentState == GameState.Victory
                ? "PLAY AGAIN"
                : "TRY AGAIN";

            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .52f, 220, 58),
                buttonText))
            {
                StartGame();
            }

            if (GUI.Button(
                new Rect(centerX - 110, Screen.height * .52f + 74, 220, 52),
                "MAIN MENU"))
            {
                MainMenu();
            }
        }
    }

    // Center text formatting
    GUIStyle CenterStyle(int size)
    {
        GUIStyle style = new GUIStyle(GUI.skin.label);

        style.fontSize = size;
        style.alignment = TextAnchor.MiddleCenter;
        style.fontStyle = FontStyle.Bold;
        style.normal.textColor = Color.white;

        return style;
    }
}
