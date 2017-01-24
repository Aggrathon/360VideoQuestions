﻿using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EditorScenario))]
public class ScenarioEditor : Editor {

	public string json;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();
		if(GUILayout.Button("Refresh JSON"))
		{
			json = (target as EditorScenario).ToJson();
		}
		EditorGUILayout.TextArea(json);
	}
}
