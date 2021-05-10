using UnityEditor;
using UnityEngine;

[CustomEditor(typeof (MeshCombiner)) ]
public class MeshCombinerEditor : Editor
{
    override
    public void OnInspectorGUI()
    {
        MeshCombiner mc = target as MeshCombiner;
        if (GUILayout.Button("Combine Meshes"))
            mc.CombineMeshes();
    }
}
