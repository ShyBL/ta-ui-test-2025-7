using UnityEditor;
using UnityEngine;
using System.IO;

public class SpriteSheetUpdater : EditorWindow
{
    private Texture2D oldSprite;
    private Texture2D newSprite;

    [MenuItem("Tools/Update Sprite Sheet")]
    public static void ShowWindow()
    {
        GetWindow<SpriteSheetUpdater>("Sprite Sheet Updater");
    }

    private void OnGUI()
    {
        GUILayout.Label("Update Sprite Sheet", EditorStyles.boldLabel);

        oldSprite = (Texture2D)EditorGUILayout.ObjectField("Old Sprite Sheet:", oldSprite, typeof(Texture2D), false);
        newSprite = (Texture2D)EditorGUILayout.ObjectField("New Sprite Sheet:", newSprite, typeof(Texture2D), false);

        if (GUILayout.Button("Replace Sprite") && oldSprite != null && newSprite != null)
        {
            ReplaceSprite();
        }
    }

    private void ReplaceSprite()
    {
        string oldPath = AssetDatabase.GetAssetPath(oldSprite);
        string newPath = AssetDatabase.GetAssetPath(newSprite);

        if (!File.Exists(newPath))
        {
            Debug.LogError("New sprite file not found: " + newPath);
            return;
        }

        if (!File.Exists(oldPath))
        {
            Debug.LogError("Old sprite file not found: " + oldPath);
            return;
        }

        // Copy new sprite over old sprite
        File.Copy(newPath, oldPath, true);
        
        // Reimport the asset to update Unity
        AssetDatabase.ImportAsset(oldPath, ImportAssetOptions.ForceUpdate);

        Debug.Log("Sprite Sheet updated successfully!");
    }
}