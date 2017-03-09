using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupPanel : MonoBehaviour {

	void Start () {
		gameObject.SetActive(false);
	}
	
	public bool Close()
	{
		if(gameObject.activeSelf)
		{
			gameObject.SetActive(false);
			return true;
		}
		else
		{
			return false;
		}
	}

	public void Toggle()
	{
		gameObject.SetActive(!gameObject.activeSelf);
	}
}
