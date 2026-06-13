using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(MapGenEditor))]
public class MapGenEditorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (target is MapGenEditor editor)
        {
            Undo.RecordObject(target, "Update Board Spaces");
            if (GUILayout.Button("Resize"))
            {
                editor.Regenerate();
            }
            if (GUILayout.Button("Output"))
            {
                editor.SaveMap();
            }
        }
    }
}
