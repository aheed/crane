using System.Collections.Generic;
using UnityEngine;

public class Chain
{
    private const int COLLIDER_BUFFER_SIZE = 8;
    private const float COLLISION_RADIUS = 0.5f; // Collision radius around each node.  Set high to avoid tunneling.
    public ChainLink[] links;
    public ChainLink claw;
    public Vector2 gravityVector = new Vector2(0f, -9.81f);
    public float simTimeFactor = 10f;
    public float linkLength = 0.5f;
    public float jakobsenIterations = 1;
    public float speedRetention = 0.99f;
    public float clawSpeedRetention = 0.9985f;
    public float clawMassRatio = 10f;
    public float maxVerticalSpeed = 5f;
    public float maxHorizontalSpeed = 5f;
    private Vector2 currentMoveInput = Vector2.zero;
    private float length;
    private float maxLength;
    private int activeLinks;
    private Vector2 topPosition;
    private float firstLinkOffsetY; //unnecessary?

    public Chain(int numLinks, Vector2 startPosition, float linkLength, float simTimeFactor, float jakobsenIterations, float speedRetention, float clawMassRatio, float clawSpeedRetention, float maxVerticalSpeed, float maxHorizontalSpeed)
    {
        links = new ChainLink[numLinks];
        this.linkLength = linkLength;
        this.simTimeFactor = simTimeFactor;
        this.jakobsenIterations = jakobsenIterations;
        this.speedRetention = speedRetention;
        this.clawMassRatio = clawMassRatio;
        this.clawSpeedRetention = clawSpeedRetention;
        topPosition = startPosition;
        maxLength = numLinks * linkLength;
        length = maxLength / 2f;
        for (int i = 0; i < numLinks; i++)
        {
            links[i] = new ChainLink(startPosition + new Vector2(i * linkLength * 0.1f, -i * linkLength));
        }        
        UpdateActiveLinkCount();
        UpdateTopLinkPosition();
        claw = new ChainLink(links[numLinks - 1].position);
    }

    public void SetMoveInput(Vector2 input)
    {
        currentMoveInput = input;
    }

    public void Update(float deltaTime)
    {
        var dt = deltaTime * simTimeFactor;
        var dtSquared = dt * dt;

        var horizontalSpeed = currentMoveInput.x * maxHorizontalSpeed;
        topPosition += new Vector2(horizontalSpeed, 0) * deltaTime;

        var verticalSpeed = currentMoveInput.y * maxVerticalSpeed;
        if (verticalSpeed != 0f)
        {
            //Debug.Log($"Vertical speed: {verticalSpeed} length: {length} / {maxLength}");
            length -= verticalSpeed * deltaTime;
            if (length > maxLength) length = maxLength;
            if (length < linkLength) length = linkLength;
            UpdateActiveLinkCount();
        }
        UpdateTopLinkPosition();

        for (int i = GetTopLinkIndex(); i < links.Length; i++) // keep the top link fixed
        {
            var link = links[i];
            var currentPosition = link.position;
            link.position += (currentPosition - link.lastPosition) * speedRetention + dtSquared * gravityVector;
            link.lastPosition = currentPosition;
        }

        var clawCurrentPosition = claw.position;
        claw.position += (clawCurrentPosition - claw.lastPosition) * clawSpeedRetention + dtSquared * gravityVector;
        claw.lastPosition = clawCurrentPosition;

        var collisions = GetCollisions();
        for (int j = 0; j < jakobsenIterations; j++)
        {
            ConstrainLinks();
            ApplyCollisions(collisions);
        }
    }

    public float Length
    {
        get { return length; }
    }

    void UpdateActiveLinkCount()
    {
        int newActiveLinksCount = (int)(length / linkLength);
        if (newActiveLinksCount != activeLinks)
        {
            //Debug.Log($"Active links changed from {activeLinks} to {newActiveLinksCount}");
            activeLinks = newActiveLinksCount;
            if (newActiveLinksCount > links.Length)
            {
                Debug.LogError("Chain length exceeded number of links!");
            }
            UpdateTopLinkPosition();
            links[GetTopLinkIndex()].lastPosition = links[GetTopLinkIndex()].position; // Prevents snapping when activating a new link
        }
    }

    void UpdateTopLinkPosition()
    {
        firstLinkOffsetY = length - (activeLinks * linkLength);
        links[GetTopLinkIndex()].position = topPosition + new Vector2(0, -firstLinkOffsetY);
    }   

    List<CircularChainCollision> GetCollisions()
    {
        Collider2D[] colliderBuffer = new Collider2D[COLLIDER_BUFFER_SIZE];
        var contactFilter = new ContactFilter2D();
        contactFilter.NoFilter();
        var collisions = new List<CircularChainCollision>();
        for (int i = GetTopLinkIndex() + 1; i < links.Length; i++) // Start from +1 to keep the top link fixed
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

    public int GetTopLinkIndex()
    {
        return links.Length - activeLinks;
    }

    void ConstrainLinks()
    {
        var firstLinkPos = links[GetTopLinkIndex()].position;
        for (int i = GetTopLinkIndex() + 1; i < links.Length; i++)
        {
            var prevLink = links[i - 1];
            var link = links[i];
            var direction = (prevLink.position - link.position).normalized;

            float dist = Vector2.Distance(link.position, prevLink.position);

            float difference = dist - linkLength;

            link.position += direction * difference * 0.4f;
            prevLink.position -= direction * difference * 0.4f;
        }
        links[GetTopLinkIndex()].position = firstLinkPos; // Re-fix the first link

        // Constrain the last link to simulate extra mass (like a claw)
        var lastLink = links[links.Length - 1];
        //var secondLastLink = links[links.Length - 2];
        var lastDirection = (lastLink.position - claw.position).normalized;
        float lastDist = Vector2.Distance(claw.position, lastLink.position);
        float lastDifference = lastDist - linkLength;
        claw.position += lastDirection * lastDifference * (1f / (1f + clawMassRatio));
        lastLink.position -= lastDirection * lastDifference * (clawMassRatio / (1f + clawMassRatio));
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