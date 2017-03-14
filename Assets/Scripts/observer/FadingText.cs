using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour {

	Text text;
	public float time = 1.0f;
	float end;

	void Start () {
		gameObject.SetActive(false);
		text = GetComponentInChildren<Text>();
	}

	public void Show(string str)
	{
		text.text = str;
		gameObject.SetActive(true);
		end = Time.time + time;
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	private void Update()
	{
		if (Time.time > end)
			gameObject.SetActive(false);
	}

}
