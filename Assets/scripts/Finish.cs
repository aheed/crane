using UnityEngine;

public class Finish : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Finish triggered by: " + other.name);
        if (!other.name.StartsWith("marble")) return;
        var gameState = GameState.GetInstance();
        gameState.SetStatus(GameStatus.FINISHED);
    }
}
