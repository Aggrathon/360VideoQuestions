using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Scenario", menuName = "Scenario")]
public class Scenario: ScriptableObject
{
	public Scene[] scenes;

	public static explicit operator RuntimeScenario(Scenario s)
	{
		RuntimeScenario rs = new RuntimeScenario();
		rs.scenes = s.scenes;
		return rs;
	}
}

[System.Serializable]
public class RuntimeScenario
{
	public Scene[] scenes;

	public static RuntimeScenario LoadJson(string json)
	{
		return JsonUtility.FromJson<RuntimeScenario>(json);
	}

	public void AddJson(string json)
	{
		RuntimeScenario other = LoadJson(json);
		CombineScenario(other);
	}

	public void CombineScenario(RuntimeScenario other)
	{
		int oldLen = scenes.Length;
		Array.Resize<Scene>(ref scenes, scenes.Length + other.scenes.Length);
		Array.Copy(other.scenes, 0, scenes, oldLen, other.scenes.Length);
	}
}