using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chain
{
    private const int COLLIDER_BUFFER_SIZE = 8;
    private const float COLLISION_RADIUS = 0.5f; // Collision radius around each node.  Set high to avoid tunneling.
    public ChainLink[] links;
    public Vector2 gravityVector = new Vector2(0f, -9.81f);
    public float simTimeFactor = 10f;
    public float linkLength = 0.5f;
    public float jakobsenIterations = 1;
    public float speedRetention = 0.99f;

    public Chain(int numLinks, Vector2 startPosition, float linkLength, float simTimeFactor, float jakobsenIterations, float speedRetention)
    {
        links = new ChainLink[numLinks];
        this.linkLength = linkLength;
        this.simTimeFactor = simTimeFactor;
        this.jakobsenIterations = jakobsenIterations;
        this.speedRetention = speedRetention;
        for (int i = 0; i < numLinks; i++)
        {
            links[i] = new ChainLink(startPosition + new Vector2(i * linkLength * 0.1f, -i * linkLength));
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

        var collisions = GetCollisions();
        for (int j = 0; j < jakobsenIterations; j++)
        {
            ConstrainLinks();
            ApplyCollisions(collisions);
        }
    }

    List<CircularChainCollision> GetCollisions()
    {
        Collider2D[] colliderBuffer = new Collider2D[COLLIDER_BUFFER_SIZE];
        var contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        var collisions = new List<CircularChainCollision>();
        for (int i = 1; i < links.Length; i++) // Start from 1 to keep the first link fixed
        {
            var overlaps = Physics2D.OverlapCircle(links[i].position, COLLISION_RADIUS, contactFilter, colliderBuffer);
            for (int j = 0; j < overlaps; j++)
            {
                var collider = colliderBuffer[j];
                collisions.Add(new CircularChainCollision { collider = collider, link = links[i], colliderRadius = collider.bounds.extents.magnitude });
            }
        }

        return collisions;
    }

    void ApplyCollisions(List<CircularChainCollision> collisions)
    {
        foreach (var collision in collisions)
        {
            var collider = collision.collider;
            var link = collision.link;

            var closestPoint = collider.ClosestPoint(link.position);
            var direction = (link.position - closestPoint).normalized;

            float dist = Vector2.Distance(link.position, closestPoint);
            var linkCollisionDistance = (linkLength * 0.5f);
            if (dist < linkCollisionDistance)
            {
                float difference = linkCollisionDistance - dist;
                link.position += direction * difference;
            }
        }
    }

    void ConstrainLinks()
    {
        var firstLinkPos = links[0].position;
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
        links[0].position = firstLinkPos; // Re-fix the first link
    }

    /*void OrientLinks()
    {
        for (int i = 1; i < links.Length; i++)
        {
            var prevLink = links[i - 1];
            var link = links[i];
            var direction = (prevLink.position - link.position).normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            // Set the rotation of the link GameObject here if needed

            link.transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }*/
}