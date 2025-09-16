using UnityEngine;

public class Marble : MonoBehaviour, IGrabbable
{
    public float grabOffsetY = 0.5f;
    private Collider2D ungrabbableCollider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ungrabbableCollider = transform.Find("visible").GetComponent<Collider2D>();
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
    }

    public void Release()
    {
        ungrabbableCollider.enabled = true;
    }

    public float GetMassRatio()
    {
        return 10.0f;
    }
}
