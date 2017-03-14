using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class UILayer : MonoBehaviour {
	
	public float buttonAngle = 40f;
	public int options = 1;

	internal Permutation permutation;

	private void Awake()
	{
		permutation = new Permutation(transform.childCount - 1, true);
	}

	void Update ()
	{
		//Reset UI rotation
#if UNITY_EDITOR
		Vector3 camRot = Camera.main.transform.eulerAngles;
#else
		Vector3 camRot = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles;
#endif
		if (camRot.x > 70f && camRot.x < 89.9f)
		{
			RecalculatePositions();
		}
	}

	public void ShowQuestion(aggrathon.vq360.data.Event question, string folderpath, Action<aggrathon.vq360.data.Option> selectionCallback)
	{
		if(question.title != "")
		{
			transform.GetChild(0).gameObject.SetActive(true);
			transform.GetChild(0).GetComponentInChildren<Text>().text = question.title;
		}
		else
		{
			transform.GetChild(0).gameObject.SetActive(false);
		}
		
		if(question.options.Length >= transform.childCount)
		{
			DebugText.LogImportant("Too many options in question '" + question.title + "' (max " + (transform.childCount-1) + ")");
			aggrathon.vq360.data.Option[] opts = new aggrathon.vq360.data.Option[transform.childCount-1];
			System.Array.Copy(question.options, opts, transform.childCount-1);
			question.options = opts;
		}

		options = question.options.Length;
		for (int i = 0; i < options; i++)
		{
			aggrathon.vq360.data.Option option = question.options[permutation.IterateNext(options)];
			transform.GetChild(i+1).GetComponent<VrButton>().Setup(
				option.text,
				option.image == "" ? "" : Path.Combine(folderpath, option.image),
				() => selectionCallback(option));
			transform.GetChild(i+1).gameObject.SetActive(true);
		}
		for (int i = question.options.Length+1; i < transform.childCount; i++)
		{
			transform.GetChild(i).gameObject.SetActive(false);
		}

		RecalculatePositions();
		gameObject.SetActive(true);
		permutation.Randomize();
	}

	public void RecalculatePositions()
	{
#if UNITY_EDITOR
		Vector3 camRot = Camera.main.transform.eulerAngles;
#else
		Vector3 camRot = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles;
#endif
		float rotation = camRot.y;
		float offset = -(options-1) * buttonAngle *0.5f;
		transform.GetChild(0).eulerAngles = new Vector3(0, rotation, 0);
		for (int i = 1; i < transform.childCount; i++)
		{
			transform.GetChild(i).eulerAngles = new Vector3(0, rotation + offset + buttonAngle * (i-1), 0);
		}
	}
}
