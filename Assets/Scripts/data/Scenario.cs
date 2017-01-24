using UnityEngine;
using System;

namespace aggrathon.vq360.data
{

	[Serializable]
	public class Scenario
	{
		public Scene[] scenes;

		public static Scenario LoadJson(string json)
		{
			return JsonUtility.FromJson<Scenario>(json);
		}

		public void AddJson(string json)
		{
			Scenario other = LoadJson(json);
			CombineScenario(other);
		}

		public void CombineScenario(Scenario other)
		{
			int oldLen = scenes.Length;
			Array.Resize<Scene>(ref scenes, scenes.Length + other.scenes.Length);
			Array.Copy(other.scenes, 0, scenes, oldLen, other.scenes.Length);
		}
	}

}