using UnityEditor;
using UnityEngine;

/// <summary>
/// Saves mesh and entire prefab from gameview, your procedural mesh prefab is saved.
/// </summary>
class CreatePrefabFromSelected
{
    const string menuName = "GameObject/Create Prefab From Selected";

    /// <summary>
    /// Adds a menu named "Create Prefab From Selected" to the GameObject menu.
    /// </summary>
    [MenuItem(menuName)]
    [System.Obsolete]
    public static void CreatePrefabMenu()
    {
        var go = Selection.activeGameObject;

        Mesh m1 = go.GetComponent<MeshFilter>().mesh;
        string meshPath = "Assets/Meshes/" + go.name + "_M" + ".asset";
        AssetDatabase.CreateAsset(m1, meshPath);

        Debug.Log("Successfully created mesh in path \"" + meshPath + "\"");

        string prefabPath = "Assets/Prefabs/" + go.name + ".prefab";
        Object prefab = EditorUtility.CreateEmptyPrefab(prefabPath);
        EditorUtility.ReplacePrefab(go, prefab);
        AssetDatabase.Refresh();
    }

    /// <summary>
    /// Validates the menu.
    /// The item will be disabled if no game object is selected.
    /// </summary>
    /// <returns>True if the menu item is valid.</returns>
    [MenuItem(menuName, true)]
    public static bool ValidateCreatePrefabMenu()
    {
        return Selection.activeGameObject != null;
    }
}
