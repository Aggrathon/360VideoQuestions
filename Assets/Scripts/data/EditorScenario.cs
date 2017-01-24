using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Scenario", menuName = "Scenario")]
public class EditorScenario: ScriptableObject
{
	[Serializable]
	public struct Image
	{
		public string name;
		public Texture2D image;
	}

	public Scene[] scenes;
	public Image[] images;

	public static explicit operator Scenario(EditorScenario s)
	{
		Scenario rs = new Scenario();
		rs.scenes = s.scenes;
		return rs;
	}

	public string ToJson()
	{
		return JsonUtility.ToJson((Scenario)this, true);
	}
}