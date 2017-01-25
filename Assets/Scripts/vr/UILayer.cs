using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class UILayer : MonoBehaviour {

	public Transform buttonHolder;
	public float buttonAngle = 40f;

	void OnEnable()
	{
		Vector3 rot = transform.rotation.eulerAngles;
		rot.y = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles.y;
		transform.eulerAngles = rot;
	}
	
	void Update () {
		//Reset UI rotation
		Vector3 camRot = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles;
		if (camRot.x > 70f && camRot.x < 89.9f)
		{
			Vector3 rot = transform.rotation.eulerAngles;
			rot.y = camRot.y;
			transform.eulerAngles = rot;
		}
	}

	public void ShowQuestion(aggrathon.vq360.data.Event question, string folderpath, Action<string> actionCallback)
	{
		if(question.title != "")
		{
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetChild(1).GetComponent<Text>().text = question.title;
		}
		else
		{
			transform.GetChild(0).gameObject.SetActive(false);
		}

		buttonHolder.eulerAngles = new Vector3(0, -(question.options.Length - 1) * buttonAngle * 0.5f, 0);

		if(question.options.Length > buttonHolder.childCount)
		{
			DebugText.LogImportant("Too many options in question '" + question.title + "' (max " + buttonHolder.childCount + ")");
			aggrathon.vq360.data.Option[] opts = new aggrathon.vq360.data.Option[buttonHolder.childCount];
			System.Array.Copy(question.options, opts, buttonHolder.childCount);
			question.options = opts;
		}

		for (int i = 0; i < question.options.Length; i++)
		{
			string action = question.options[i].action;
			buttonHolder.GetChild(i).GetComponent<VrButton>().Setup(
				question.options[i].text,
				question.options[i].image == "" ? "" : Path.Combine(folderpath, question.options[i].image),
				() => actionCallback(action));
			buttonHolder.GetChild(i).gameObject.SetActive(true);
		}
		for (int i = question.options.Length; i < buttonHolder.childCount; i++)
		{
			buttonHolder.GetChild(i).gameObject.SetActive(false);
		}

		gameObject.SetActive(true);
	}
}
