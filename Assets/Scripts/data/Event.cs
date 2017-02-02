
using System.Text;
using UnityEngine;

namespace aggrathon.vq360.data
{

	[System.Serializable]
	public class Event
	{
		public float time;
		public string action;
		public string title;
		public Option[] options;

		public void ScrambleOptions()
		{
			for (int i = 0; i < options.Length; i++)
			{
				int n = Random.Range(i, options.Length);
				if(i != n)
				{
					Option temp = options[i];
					options[i] = options[n];
					options[n] = temp; 
				}
			}
		}

		public string GetOptionText()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("Options:");
			for (int i = 0; i < options.Length; i++)
			{
				sb.Append(" ");
				sb.Append(options[i].GetName());
			}
			return sb.ToString();
		}
	}

	[System.Serializable]
	public struct Option
	{
		public string image;
		public string text;
		public string action;

		public string GetName()
		{
			if (text != "")
			{
				return text;
			}
			else if (image != "")
			{
				return image;
			}
			else
			{
				return action;
			}
		}
	}

}