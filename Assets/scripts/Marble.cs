using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class Marble : MonoBehaviour, IGrabbable
{
    public float grabOffsetY = 0.5f;
    public Vector2 startPosition = Vector2.zero;
    private Collider2D ungrabbableCollider;
    private Rigidbody2D rb;
    private bool grabbed = false;
    private Transform glareTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Reset();
        glareTransform = transform.Find("marble_glare");
    }

    void Update()
    {
        if (grabbed)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (glareTransform)
        {
            glareTransform.rotation = Quaternion.Euler(0f, 0f, 0f);
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

    [ContextMenu("Set Start Position")]
    public void SetStartPosition()
    {
#if UNITY_EDITOR
        Undo.RegisterFullObjectHierarchyUndo(gameObject, "Set Start Position");
        startPosition = transform.position;
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(transform);
#endif        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Marble))]
class MarbleEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            GUILayout.Space(6);
            if (GUILayout.Button("Set Start Position"))
                ((Marble)target).SetStartPosition();
        }
    }
}
#endif

