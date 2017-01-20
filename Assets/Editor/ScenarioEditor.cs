using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Scenario))]
public class ScenarioEditor : Editor {

	public string json;

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();
		EditorGUILayout.Space();
		if(GUILayout.Button("Refresh JSON"))
		{
			json = JsonUtility.ToJson((target as Scenario), true);
		}
		EditorGUILayout.TextArea(json);
	}
}
