using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class DebugText : MonoBehaviour {

	private static DebugText instance;
	private Text text;

	void Awake()
	{
		text = GetComponent<Text>();
		instance = this;
	}

	static public void Log(string str)
	{
		if(instance != null)
			instance.text.text += "\n\n"+ str;
	}

	void OnDestroy()
	{
		if (instance == this)
			instance = null;
	}
}
