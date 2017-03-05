using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour {

	private static DebugText instance;
	private Text text;
	private string[] logs;
	private int logCount;
	private int logStart;

	public bool showOnlyImportant = true;
	[SerializeField] int logLength = 5;

	void Awake()
	{
		text = GetComponent<Text>();
		instance = this;
		logs = new string[logLength];
		logCount = 0;
		logStart = 0;
		text.text = "";
	}

	static public void Log(string str)
	{
		Debug.Log(str);
		if (instance != null && !instance.showOnlyImportant)
			instance.AddText(str);
	}

	static public void LogImportant(string str)
	{
		Debug.Log(str);
		if (instance != null)
			instance.AddText(str);
	}

	static public void LogError(string str)
	{
		Debug.LogError(str);
		if (instance != null)
			instance.AddText("<color=red>" + str +"</color>");
	}

	static public void LogException(string str, Exception e)
	{
		LogError(string.Format("{0} ({1})", str, e.GetType().ToString()));
		Debug.LogException(e);
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}

	void AddText(string txt)
	{
		if(logCount == logLength)
		{
			logStart = (logStart + 1) % logLength;
		}
		else
		{
			logCount++;
		}
		int end = (logStart + logCount) % logLength;
		logs[end] = txt;
		if (end != logCount+logStart)
		{
			text.text = string.Join("\n\n", logs, logStart, logCount-end) + "\n\n" + string.Join("\n\n", logs, 0, end+1);
		}
		else
		{
			text.text = string.Join("\n\n", logs, logStart, logCount);
		}
	}
}
