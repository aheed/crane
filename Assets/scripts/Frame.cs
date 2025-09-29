using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
[DisallowMultipleComponent]
public class Frame : MonoBehaviour
{
    [Tooltip("If empty, will try to use Camera.main or an object tagged 'MainCamera'.")]
    public Camera sourceCamera;

    GameObject bottom;
    GameObject left;
    GameObject right;

    void Start()
    {
        bottom = transform.Find("Bottom").gameObject;
        left = transform.Find("Left").gameObject;
        right = transform.Find("Right").gameObject;
    }

    Camera ResolveCamera()
    {
        if (sourceCamera) return sourceCamera;
        if (Camera.main) return Camera.main;
        var go = GameObject.FindWithTag("MainCamera");
        return go ? go.GetComponent<Camera>() : null;
    }

    [ContextMenu("Bake From Camera")] 
    public void BakeNow()
    {
        var cam = ResolveCamera();
        if (!cam)
        {
            Debug.LogWarning($"[{nameof(Frame)}] No camera found. Assign one or tag a camera 'MainCamera'.");
            return;
        }
#if UNITY_EDITOR
        ApplyFrom(cam, recordUndo: true, recordPrefabMods: true, markDirty: true);
#else
        ApplyFrom(cam, false, false, false);
#endif
    }

    void ApplyFrom(Camera cam, bool recordUndo, bool recordPrefabMods, bool markDirty)
    {
#if UNITY_EDITOR
        if (recordUndo) Undo.RegisterFullObjectHierarchyUndo(gameObject, "Bake From Camera");
#endif
        float viewportHeight = cam.orthographicSize * 2f;
        float viewportWidth = viewportHeight * cam.aspect;

        bottom.transform.localScale = new Vector3(viewportWidth, 1f, 1f);
        bottom.transform.localPosition = new Vector3(0f, -viewportHeight / 2f, 0f);
        left.transform.localScale = new Vector3(1f, viewportHeight, 1f);
        left.transform.localPosition = new Vector3(-viewportWidth / 2f, 0f, 0f);
        right.transform.localScale = new Vector3(1f, viewportHeight, 1f);
        right.transform.localPosition = new Vector3(viewportWidth / 2f, 0f, 0f);

#if UNITY_EDITOR
        if (recordPrefabMods)
        {
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        }
        if (markDirty)
        {
            EditorUtility.SetDirty(this);
            EditorUtility.SetDirty(transform);
        }
#endif
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(Frame))]
class CameraDrivenBakerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            GUILayout.Space(6);
            if (GUILayout.Button("Bake From Main Camera"))
                ((Frame)target).BakeNow();
        }
    }
}
#endif
