using UnityEditor;
using UnityEngine;

public class CreateMinigame : EditorWindow
{
    private string id = "";
    private string name = "";
    private string description = "";

    [MenuItem("Tools/MiniGames/Create Minigame")] 
    public static void ShowWindow()
    {
        var window = GetWindow<CreateMinigame>(true, "Create Minigame");
        window.minSize = new Vector2(420, 180);
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("Create New Minigame", EditorStyles.boldLabel);

        id = EditorGUILayout.TextField(new GUIContent("Id", "Id of Minigame. Will be inserted into enum"), id);
        name = EditorGUILayout.TextField(new GUIContent("Name"), name);
        description = EditorGUILayout.TextField(new GUIContent("Description"), description);

        GUILayout.Space(8);

        if (GUILayout.Button("Create", GUILayout.Height(32)))
        {
            MinigameCreator.Create(id, name, description);
        }
    }
}


