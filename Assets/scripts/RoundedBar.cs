using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class RoundedBar : MonoBehaviour
{
    [ContextMenu("Set Edge Positions")]
    public void SetEdgePositions()
    {
        var bar = transform.Find("SquareObstacle");
        var leftEdge = transform.Find("CircleObstacleL");
        var rightEdge = transform.Find("CircleObstacleR");
        if (leftEdge == null || rightEdge == null)
        {
            Debug.LogWarning($"[{nameof(RoundedBar)}] Could not find left_edge or right_edge child objects.");
            return;
        }
#if UNITY_EDITOR
        Undo.RegisterFullObjectHierarchyUndo(gameObject, "Set Edge Positions");

        float edgeOffsetX = bar.localScale.x / 2f;
        leftEdge.localPosition = new Vector3(-edgeOffsetX, 0f, 0f);
        rightEdge.localPosition = new Vector3(edgeOffsetX, 0f, 0f);
        
        PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        PrefabUtility.RecordPrefabInstancePropertyModifications(transform);
        EditorUtility.SetDirty(this);
        EditorUtility.SetDirty(transform);
#endif        
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(RoundedBar))]
class RoundedBarEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        using (new EditorGUI.DisabledScope(Application.isPlaying))
        {
            GUILayout.Space(6);
            if (GUILayout.Button("Set Edge Positions"))
                ((RoundedBar)target).SetEdgePositions();
        }
    }
}
#endif

