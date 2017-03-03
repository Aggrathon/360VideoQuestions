using UnityEngine;
using System;

namespace aggrathon.vq360.data
{

	[Serializable]
	public class Scenario
	{
		public string[] start;
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
			oldLen = start.Length;
			Array.Resize<string>(ref start, start.Length + other.start.Length);
			Array.Copy(other.start, 0, start, oldLen, other.start.Length);
		}

		public void InitScenes()
		{
			if(start == null || start.Length == 0)
			{
				start = new string[] { scenes[0].name };
			}
			else if (start.Length > 1)
			{
				for (int i = 0; i < start.Length; i++)
				{
					string temp = start[i];
					int rnd = UnityEngine.Random.Range(i, start.Length - 1);
					start[i] = start[rnd];
					start[rnd] = temp;
				}
			}
		}
	}

}