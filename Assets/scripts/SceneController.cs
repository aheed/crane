using UnityEngine;

public class SceneController : MonoBehaviour
{
    GameState gameState;

    void Start()
    {
        gameState = GameState.GetInstance();
        gameState.Subscribe(GameEvent.START, OnStartCallback);
        gameState.Subscribe(GameEvent.RESTART_REQUESTED, OnRestartRequestCallback);
        StartNewGame();
    }

    void Update()
    {
        GameStateContents stateContents = gameState.GetStateContents();

        if (stateContents.gameStatus == GameStatus.DEAD ||
                 stateContents.gameStatus == GameStatus.FINISHED)
        {
            gameState.UpdateRestartTimer(Time.deltaTime);
        }
    }

    void StartNewGame()
    {
        gameState = FindAnyObjectByType<GameState>();

        gameState.GetStateContents().homeButtonVisible = true;
        gameState.ReportEvent(GameEvent.HOME_BUTTON_UPDATED);
        gameState.GetStateContents().pauseButtonVisible = true;
        gameState.ReportEvent(GameEvent.PAUSE_BUTTON_UPDATED);

        gameState.Reset();
        gameState.SetStatus(GameStatus.PLAYING);
        gameState.ReportEvent(GameEvent.START);
        gameState.SetPause(false);
    }

    private void OnStartCallback()
    {
        Debug.Log("Game started");
    }
    
    private void OnRestartRequestCallback()
    {
        if (!gameState.IsRestartAllowed())
        {
            Debug.Log("Too early to restart");
            return;
        }

        Debug.Log("Starting a new game");
        StartNewGame();
    }
}
