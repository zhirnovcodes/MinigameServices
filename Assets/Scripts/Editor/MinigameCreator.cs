using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class MinigameCreator
{
    public static void Create(string id, string name, string description)
    {
        AddEnumValue(id);
        UpdateConfig(id, name, description);
        var newRoot = CopyTemplate(id);
        var scenePath = FindOrCreateMainScene(newRoot, id);
        OpenScene(scenePath);
        AddModelGameObject(id);
        RenameTemplateModelScript(newRoot, id);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        RebuildAddressables();
    }

    private static void AddEnumValue(string id)
    {
        var enumPath = FindMinigamesEnumPath();
        var content = File.ReadAllText(enumPath);
        var insertIndex = content.LastIndexOf('}');
        var toInsert = ",\n    " + id;
        content = content.Insert(insertIndex - 0, toInsert);
        File.WriteAllText(enumPath, content);
        AssetDatabase.ImportAsset(ToAssetPath(enumPath));
    }

    private static string FindMinigamesEnumPath()
    {
        var guid = AssetDatabase.FindAssets("t:Script Minigames").FirstOrDefault();
        if (!string.IsNullOrEmpty(guid))
        {
            return AssetDatabase.GUIDToAssetPath(guid);
        }
        return "Assets/Scripts/Enums/MiniGames.cs";
    }

    private static void UpdateConfig(string id, string name, string description)
    {
        var config = AssetDatabase.LoadAssetAtPath<MinigamesConfig>("Assets/Content/Local/Configs/Minigames/MinigamesConfig.asset");
        if (config == null)
        {
            config = ScriptableObject.CreateInstance<MinigamesConfig>();
            Directory.CreateDirectory("Assets/Content/Local/Configs/Minigames");
            AssetDatabase.CreateAsset(config, "Assets/Content/Local/Configs/Minigames/MinigamesConfig.asset");
        }
        if (config.Minigames == null)
        {
            config.Minigames = new System.Collections.Generic.List<MinigameConfig>();
        }
        var item = new MinigameConfig();
        var type = item.GetType();
        var idField = type.GetField("Id");
        var nameField = type.GetField("Name");
        var descField = type.GetField("Description");
        if (idField != null) idField.SetValue(item, id);
        if (nameField != null) nameField.SetValue(item, name);
        if (descField != null) descField.SetValue(item, description);
        config.Minigames.Add(item);
        EditorUtility.SetDirty(config);
    }

    private static string CopyTemplate(string id)
    {
        var source = "Assets/Template";
        if (!AssetDatabase.IsValidFolder(source))
        {
            source = "Assets/Content/Remote/Minigames/Wheel";
        }
        var target = $"Assets/Content/Remote/Minigames/{id}";
        var parent = "Assets/Content/Remote/Minigames";
        if (!AssetDatabase.IsValidFolder(parent))
        {
            AssetDatabase.CreateFolder("Assets/Content/Remote", "Minigames");
        }
        if (AssetDatabase.IsValidFolder(target))
        {
            AssetDatabase.DeleteAsset(target);
        }
        AssetDatabase.CopyAsset(source, target);
        AssetDatabase.Refresh();
        return target;
    }

    private static void RenameTemplateModelScript(string newRoot, string id)
    {
        var scripts = Directory.GetFiles(newRoot + "/Scripts", "*.cs", SearchOption.AllDirectories);
        foreach (var path in scripts)
        {
            var text = File.ReadAllText(path);
            if (text.Contains("TemplateMinigameModel"))
            {
                text = text.Replace("TemplateMinigameModel", id + "MinigameModel");
                File.WriteAllText(path, text);
                var newName = Path.Combine(Path.GetDirectoryName(path), id + "MinigameModel.cs");
                AssetDatabase.RenameAsset(ToAssetPath(path), id + "MinigameModel.cs");
                AssetDatabase.ImportAsset(ToAssetPath(newName));
                break;
            }
        }
    }

    private static string FindOrCreateMainScene(string newRoot, string id)
    {
        var scenePath = Path.Combine(newRoot, "main.unity");
        if (File.Exists(scenePath))
        {
            return ToAssetPath(scenePath);
        }
        var assetPath = ToAssetPath(Path.Combine(newRoot, "main.unity"));
        var scene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, assetPath);
        return assetPath;
    }

    private static void OpenScene(string sceneAssetPath)
    {
        EditorSceneManager.OpenScene(sceneAssetPath, OpenSceneMode.Single);
    }

    private static void AddModelGameObject(string id)
    {
        var go = new GameObject(id + "Minigame");
        var typeName = id + "MinigameModel";
        var type = System.AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .FirstOrDefault(t => t.Name == typeName);
        if (type != null)
        {
            go.AddComponent(type);
        }
        EditorSceneManager.MarkSceneDirty(go.scene);
    }

    private static void RebuildAddressables()
    {
        var settings = AddressableAssetSettingsDefaultObject.Settings;
        if (settings != null)
        {
            AddressableAssetSettings.BuildPlayerContent();
        }
    }

    private static string ToAssetPath(string path)
    {
        var result = path.Replace('\\', '/');
        if (!result.StartsWith("Assets"))
        {
            var i = result.IndexOf("Assets/");
            if (i >= 0) result = result.Substring(i);
        }
        return result;
    }
}


