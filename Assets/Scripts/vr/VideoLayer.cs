using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoLayer : MonoBehaviour {

	VideoPlayer player;
	Action<string> onEnding;
	string action;
	
	void Awake () {
		player = GetComponent<VideoPlayer>();
		player.loopPointReached += OnLoopPoint;
		player.prepareCompleted += Prepared;
		player.errorReceived += OnError;
	}

	public void SetVideo(string path, string ending, Action<string> actionHandler, float delay)
	{
		if (gameObject.activeSelf)
			StopAllCoroutines();
		else
			gameObject.SetActive(true);
		
		player.url = new Uri(path, UriKind.Absolute).AbsoluteUri;
		player.Prepare();

		if (ending == "loop")
		{
			player.isLooping = true;
		}
		else
		{
			player.isLooping = false;
			if (ending != "")
			{
				action = ending;
				onEnding = actionHandler;
			}
		}
	}

	public void ClearAction()
	{
		onEnding = null;
		action = "";
	}

	void Prepared(VideoPlayer vp)
	{
		vp.Play();
	}

	void OnError(VideoPlayer cp, string e)
	{
		DebugText.LogError("Error playing video: " + e);
	}

	void OnLoopPoint(VideoPlayer vp)
	{
		if (onEnding != null)
			onEnding(action);
	}

	void OnDisable()
	{
		ClearAction();
		player.Pause();
	}

	public void Hide(float speed)
	{
		ClearAction();
		StartCoroutine(Utils.RunLater(() => gameObject.SetActive(false), new WaitForSeconds(speed)));
	}
}
