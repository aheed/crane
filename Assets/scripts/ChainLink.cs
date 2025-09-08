using UnityEngine;

public class ChainLink
{
    public Vector2 position;
    public Vector2 lastPosition;

    public ChainLink(Vector2 pos)
    {
        position = pos;
        lastPosition = pos;
    }
}