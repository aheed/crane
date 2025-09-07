using UnityEngine;
using UnityEngine.InputSystem;

public class ChainLink
{
    public Vector3 position;
    public Vector3 lastPosition;

    public ChainLink(Vector3 pos)
    {
        position = pos;
        lastPosition = pos;
    }
}

public class Chain
{
    public ChainLink[] links;
    public Vector3 gravityVector = new Vector3(0f, -9.81f, 0f);
    public float simTimeFactor = 10f;
    public float linkLength = 0.5f;
    public float jakobsenIterations = 1;
    public float speedRetention = 0.99f;

    public Chain(int numLinks, Vector3 startPosition, float linkLength, float simTimeFactor, float jakobsenIterations, float speedRetention)
    {
        links = new ChainLink[numLinks];
        this.linkLength = linkLength;
        this.simTimeFactor = simTimeFactor;
        this.jakobsenIterations = jakobsenIterations;
        this.speedRetention = speedRetention;
        for (int i = 0; i < numLinks; i++)
        {
            links[i] = new ChainLink(startPosition + new Vector3(i * linkLength * 0.1f, -i * linkLength, 0));
        }
    }

    public void Update(float deltaTime)
    {
        //var a = Mouse.current.position.ReadValue();
        var b = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        var mousePosition = new Vector3(b.x, b.y, 0);
        //var c = links[0].position;
        //Debug.Log($"{a} {b} {c}");

        links[0].position = mousePosition; // Fix the first link to the mouse position

        var dt = deltaTime * simTimeFactor;
        var dtSquared = dt * dt;

        for (int i = 1; i < links.Length; i++) // Start from 1 to keep the first link fixed
        {
            var link = links[i];
            var currentPosition = link.position;
            link.position += (currentPosition - link.lastPosition) * speedRetention + dtSquared * gravityVector;
            link.lastPosition = currentPosition;
        }

        for (int j = 0; j < jakobsenIterations; j++)
        {
            ConstrainLinks();
        }
    }

    void ConstrainLinks()
    {
        for (int i = 1; i < links.Length; i++)
        {
            var prevLink = links[i - 1];
            var link = links[i];
            var direction = (prevLink.position - link.position).normalized;

            float dist = Vector2.Distance(link.position, prevLink.position);

            float difference = dist - linkLength;

            link.position += direction * difference * 0.4f;
            prevLink.position -= direction * difference * 0.4f;
        }
    }
}