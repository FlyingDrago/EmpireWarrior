using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MissingScriptsRemover : EditorWindow
{
    [MenuItem("Tools/Missing Scripts Remover")]
    public static void ShowWindow()
    {
        GetWindow<MissingScriptsRemover>("Missing Scripts Remover");
    }

    private void OnGUI()
    {
        GUILayout.Label("Remove Missing Scripts", EditorStyles.boldLabel);

        if (GUILayout.Button("Find and Remove in Current Scene"))
        {
            RemoveInCurrentScene();
        }

        if (GUILayout.Button("Find and Remove in All Prefabs"))
        {
            RemoveInAllPrefabs();
        }

        if (GUILayout.Button("Find and Remove in Entire Project"))
        {
            RemoveInEntireProject();
        }
    }

    private static void RemoveInCurrentScene()
    {
        GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
        int count = 0;

        foreach (GameObject go in rootObjects)
        {
            count += RemoveMissingScriptsRecursive(go);
        }

        Debug.Log($"Removed {count} missing scripts from current scene");
    }

    private static void RemoveInAllPrefabs()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab");
        int totalCount = 0;

        foreach (string prefabGuid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(prefabGuid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                int count = RemoveMissingScriptsRecursive(prefab);
                if (count > 0)
                {
                    Debug.Log($"Removed {count} missing scripts from prefab: {path}");
                    EditorUtility.SetDirty(prefab);
                    totalCount += count;
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Total removed {totalCount} missing scripts from all prefabs");
    }

    private static void RemoveInEntireProject()
    {
        RemoveInCurrentScene();
        RemoveInAllPrefabs();
    }

    private static int RemoveMissingScriptsRecursive(GameObject gameObject)
    {
        int count = 0;

        // Check components on this GameObject
        count += GameObjectUtility.RemoveMonoBehavioursWithMissingScript(gameObject);

        // Recursively check children
        foreach (Transform child in gameObject.transform)
        {
            count += RemoveMissingScriptsRecursive(child.gameObject);
        }

        return count;
    }
}