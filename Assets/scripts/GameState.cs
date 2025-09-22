using System;
using UnityEngine;

public enum GameEvent
{
    START,
    RESTART_REQUESTED,
    RESTART_TIMER_EXPIRED,
    GAME_STATUS_CHANGED,
    DEBUG_ACTION1,
    DEBUG_ACTION2,
    DEBUG_ACTION3,
    DEBUG_INFO_VISIBILITY_UPDATED,
    HOME_BUTTON_UPDATED,
    PAUSE_BUTTON_UPDATED,
    SPACER_BUTTONS_UPDATED,
    FULLSCREEN_BUTTON_UPDATED,
    TOUCH_SCREEN_DETECTED,
    PAUSE_GAME,
    UNPAUSE_GAME,
    HAPPY_TIME,
    SCORE_CHANGED
}


public class GameStateContents
{
    public GameStatus gameStatus = GameStatus.PLAYING;
    public float restartCoolDownSeconds = 0f;
    public bool homeButtonVisible = false;
    public bool pauseButtonVisible = false;
    public bool spacerButtonsVisible = false;
    public bool fullScreenButtonVisible = true;
    public bool debugInfoVisible = false;
    public int score = 0;
}

public class GameState : MonoBehaviour
{
    public static float minRestartWaitSeconds = 2.0f;
    GameStateContents gameStateContents = new GameStateContents();
    public GameStateContents GetStateContents() => gameStateContents;
    static GameState singletonInstance;
    private EventPubSubNoArg pubSub = new();

    public void SetPause(bool paused)
    {
        Time.timeScale = paused ? 0f : 1f;
        ReportEvent(GameEvent.PAUSE_BUTTON_UPDATED);
    }

    public bool IsPaused()
    {
        return Time.timeScale == 0f;
    }

    public void Subscribe(GameEvent gameEvent, Action callback)
    {
        pubSub.Subscribe(gameEvent, callback);
    }

    public void Unsubscribe(GameEvent gameEvent, Action callback)
    {
        pubSub.Unsubscribe(gameEvent, callback);
    }

    public void SetDebugInfoVisible(bool visible)
    {
        gameStateContents.debugInfoVisible = visible;
        ReportEvent(GameEvent.DEBUG_INFO_VISIBILITY_UPDATED);
    }

    public static GameState GetInstance()
    {
        if (singletonInstance == null)
        {
            singletonInstance = FindAnyObjectByType<GameState>();
        }
        return singletonInstance;
    }

    public void SetStatus(GameStatus gameStatus)
    {
        if (gameStatus == gameStateContents.gameStatus)
        {
            return;
        }

        gameStateContents.gameStatus = gameStatus;
        Debug.Log($"New State: {gameStatus}");

        if (gameStatus == GameStatus.DEAD ||
           gameStatus == GameStatus.FINISHED)
        {
            gameStateContents.restartCoolDownSeconds = minRestartWaitSeconds;
        }

        pubSub.Publish(GameEvent.GAME_STATUS_CHANGED);
    }

    public void ReportEvent(GameEvent gameEvent)
    {
        pubSub.Publish(gameEvent);
    }

    public void ReportDebugEvent(GameEvent gameEvent)
    {
        if (!gameStateContents.debugInfoVisible)
        {
            return;
        }

        pubSub.Publish(gameEvent);
    }

    public bool IsGameOver()
    {
        return gameStateContents.gameStatus == GameStatus.DEAD;
    }

    public void Reset()
    {
        gameStateContents.gameStatus = GameStatus.PLAYING;
        gameStateContents.score = 0;
        gameStateContents.restartCoolDownSeconds = 0f;
    }

    public void AddScore(int score)
    {
        gameStateContents.score += score;
        ReportEvent(GameEvent.SCORE_CHANGED);
    }

    public void UpdateRestartTimer(float deltaTime)
    {
        if (gameStateContents.restartCoolDownSeconds > 0f)
        {
            gameStateContents.restartCoolDownSeconds -= deltaTime;
            if (gameStateContents.restartCoolDownSeconds <= 0f)
            {
                gameStateContents.restartCoolDownSeconds = 0f;
                ReportEvent(GameEvent.RESTART_TIMER_EXPIRED);
            }
        }
    }

    public bool IsRestartAllowed()
    {
        return (gameStateContents.gameStatus == GameStatus.DEAD ||
                gameStateContents.gameStatus == GameStatus.FINISHED) &&
                gameStateContents.restartCoolDownSeconds <= 0f;
    }
}
