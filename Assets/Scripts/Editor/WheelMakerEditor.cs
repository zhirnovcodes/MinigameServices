using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WheelMaker))]
public class WheelMakerEditor : Editor
{
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector();

		WheelMaker maker = (WheelMaker)target;
		EditorGUILayout.Space();
		if (GUILayout.Button("Build"))
		{
			maker.Build();
		}
	}
}


