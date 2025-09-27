using UnityEngine;

public class Marble : MonoBehaviour, IGrabbable
{
    public float grabOffsetY = 0.5f;
    public Vector2 startPosition = Vector2.zero;
    private Collider2D ungrabbableCollider;
    private Rigidbody2D rb;
    private bool grabbed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Reset();
    }

    void Update()
    {
        if (grabbed)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
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

    void UpdateGrabbedLayer()
    {
        var ungrabbableObject = transform.Find("marble_visible").gameObject;
        if (grabbed)
        {
            ungrabbableObject.layer = LayerMask.NameToLayer("grabbed");
        }
        else
        {
            ungrabbableObject.layer = LayerMask.NameToLayer("Default");
        }
    }

    public void Grab()
    {
        grabbed = true;
        UpdateGrabbedLayer();
    }

    public void Release(Vector2 velocity)
    {
        grabbed = false;
        UpdateGrabbedLayer();
        rb.linearVelocity = velocity;        
    }

    public float GetMassRatio()
    {
        return 10.0f;
    }
}
