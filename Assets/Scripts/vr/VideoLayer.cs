using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoLayer : MonoBehaviour {

	VideoPlayer player;
	Action<string> onEnding;
	Action onLoaded;
	string action;
	float timeToStart;
	
	void Awake () {
		player = GetComponent<VideoPlayer>();
		player.loopPointReached += OnLoopPoint;
		player.prepareCompleted += Prepared;
		player.errorReceived += OnError;
		player.frameDropped += OnDropped;
		player.started += OnStarted;
	}

	public void SetVideo(string path, string ending, Action<string> actionHandler, float delay, Action onLoaded)
	{
		if (gameObject.activeSelf)
			StopAllCoroutines();
		gameObject.SetActive(true);

		DebugText.Log("Loading Video File");
		player.url = new Uri(path, UriKind.Absolute).AbsoluteUri;
		player.Play();
		
		this.onLoaded = onLoaded;
		this.timeToStart = Time.time + delay;

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
		DebugText.Log("Video Loaded");
		if (Time.time > this.timeToStart)
		{
			if (onLoaded != null) onLoaded();
			vp.Play();
		}
		else
		{
			vp.Stop();
			StartCoroutine(Utils.RunLater(() => {
				if (onLoaded != null) onLoaded();
				vp.Play();
			}, timeToStart - Time.time));
		}
	}

	void OnError(VideoPlayer cp, string e)
	{
		DebugText.LogError("Error playing video: " + e);
	}

	void OnLoopPoint(VideoPlayer vp)
	{
		DebugText.Log("Video reached its end");
		if (onEnding != null)
			onEnding(action);
	}

	void OnStarted(VideoPlayer vp) {
		DebugText.Log("Video Started Playing");
	}

	void OnDropped(VideoPlayer vp) {
		DebugText.Log("Video Frame Dropped");
	}

	void OnDisable()
	{
		ClearAction();
		player.Stop();
	}

	public void Hide(float speed)
	{
		ClearAction();
		StartCoroutine(Utils.RunLater(() => gameObject.SetActive(false), new WaitForSeconds(speed)));
	}
}
