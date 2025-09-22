using UnityEngine;

public class Finish : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Finish triggered by: " + other.name);
        if (!other.name.StartsWith("marble")) return;
        var gameState = GameState.GetInstance();
        gameState.SetStatus(GameStatus.FINISHED);
        gameState.ReportEvent(GameEvent.HAPPY_TIME);
    }
}
