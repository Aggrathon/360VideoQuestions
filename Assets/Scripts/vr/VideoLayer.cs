using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoLayer : MonoBehaviour {

	public bool extendedLogging = false;

	VideoPlayer player;
	Action<string> onEnding;
	string action;
	
	void Awake () {
		player = GetComponent<VideoPlayer>();
		player.loopPointReached += OnLoopPoint;
		player.prepareCompleted += Prepared;
		player.errorReceived += OnError;

		if (extendedLogging)
		{
			player.frameDropped += (vp) =>
			{
				DebugText.Log("Video Frame Dropped");
			};
			player.started += (vp) =>
			{
				DebugText.Log("Video Started Playing");
			};

			DebugText.Log("Video Player setup");
		}
	}

	public void SetVideo(string path, string ending, Action<string> actionHandler, float delay)
	{
		if (gameObject.activeSelf)
			StopAllCoroutines();
		else
			gameObject.SetActive(true);

		if (extendedLogging)
			DebugText.Log("Video File recieved");

		player.url = new Uri(path, UriKind.Absolute).AbsoluteUri;
		player.Play();

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
		if (extendedLogging)
			DebugText.Log("Video Prepared");
		vp.Play();
	}

	void OnError(VideoPlayer cp, string e)
	{
		DebugText.LogError("Error playing video: " + e);
	}

	void OnLoopPoint(VideoPlayer vp)
	{
		if (extendedLogging)
			DebugText.Log("Video reached its end");
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
