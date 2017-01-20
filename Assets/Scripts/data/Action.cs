
[System.Serializable]
public class Action
{
	public float time;
	public string action;
	public string title;
	public Option[] options;
}

[System.Serializable]
public struct Option
{
	public string image;
	public string text;
	public string action;
}