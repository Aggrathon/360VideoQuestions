using UnityEngine;
using System;

namespace aggrathon.vq360.data
{

	[CreateAssetMenu(fileName = "Scenario", menuName = "Scenario")]
	public class EditorScenario : ScriptableObject
	{
		[Serializable]
		public struct Image
		{
			public string name;
			public Texture2D image;
		}

		public string[] start;
		public Scene[] scenes;
		public Image[] images;
		[TextArea(3, 15)]
		public string instructions;

		public static explicit operator Scenario(EditorScenario s)
		{
			Scenario rs = new Scenario();
			rs.scenes = s.scenes;
			rs.start = s.start;
			return rs;
		}

		public string ToJson()
		{
			return JsonUtility.ToJson((Scenario)this, true);
		}
	}

}