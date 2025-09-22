using UnityEngine;

public class Marble : MonoBehaviour, IGrabbable
{
    public float grabOffsetY = 0.5f;
    public Vector2 startPosition = Vector2.zero;
    private Collider2D ungrabbableCollider;
    private Rigidbody2D rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Reset();
    }

    public void Reset()
    {
        ungrabbableCollider = transform.Find("marble_visible").GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        rb.rotation = 0f;
        rb.constraints = RigidbodyConstraints2D.None;
        transform.position = startPosition;
        ungrabbableCollider.enabled = true;
    }

    public GameObject GetGrabbedObject()
    {
        return gameObject;
    }

    public Vector3 GetGrabOffset()
    {
        return new Vector3(0, grabOffsetY, 0);
    }

    public void Grab()
    {
        ungrabbableCollider.enabled = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public void Release(Vector2 velocity)
    {
        ungrabbableCollider.enabled = true;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.linearVelocity = velocity;
    }

    public float GetMassRatio()
    {
        return 10.0f;
    }
}
