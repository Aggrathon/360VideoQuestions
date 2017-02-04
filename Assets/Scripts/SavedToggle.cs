using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SavedToggle : MonoBehaviour {

	public bool defaultValue;
	public string saveName;

	void Start () {
		int prefVale = PlayerPrefs.GetInt(saveName, defaultValue ? 1 : 0);
		Toggle tog = GetComponent<Toggle>();
		tog.isOn = prefVale != 0;
		tog.onValueChanged.AddListener(SaveState);
	}

	void SaveState(bool state)
	{
		PlayerPrefs.SetInt(saveName, state ? 1 : 0);
	}
}
