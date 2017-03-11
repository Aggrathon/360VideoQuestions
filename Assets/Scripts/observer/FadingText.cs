using UnityEngine;
using UnityEngine.UI;

public class FadingText : MonoBehaviour {

	Text text;
	public float time = 5.0f;
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

	private void Update()
	{
		if (Time.time > end)
			gameObject.SetActive(false);
	}

}
