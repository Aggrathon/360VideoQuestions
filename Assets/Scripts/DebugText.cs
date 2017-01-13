using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour {

	private static DebugText instance;
	private Text text;

	public bool showOnlyImportant = true;

	void Awake()
	{
		text = GetComponent<Text>();
		instance = this;
	}

	static public void Log(string str)
	{
		Debug.Log(str);
		if (instance != null && !instance.showOnlyImportant)
			instance.text.text += "\n\n" + str;
	}

	static public void LogImportant(string str)
	{
		Debug.Log(str);
		if (instance != null)
			instance.text.text += "\n\n" + str;
	}

	static public void LogError(string str)
	{
		Debug.LogError(str);
		if (instance != null)
			instance.text.text += "\n\n<color=red>" + str +"</color>";
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}
}
