﻿using System;
using System.Collections;
using System.IO;
using UnityEngine;
using aggrathon.vq360.data;

public class ScenarioManager : MonoBehaviour {

	public AppStateManager stateManager;
	public ColorLayer colorLayer;
	public UILayer uiLayer;
	public float sceneChangeSpeed = 0.5f;

	Scenario scenario;
	Scene currentScene;
	string scenarioFolder;

	public void LoadScenario(string folder)
	{
		scenarioFolder = folder;
		scenario = null;
		try {
			var files = Directory.GetFiles(folder);
			bool nojson = true;
			foreach (string f in files)
			{
				if (Path.GetExtension(f) == ".json")
				{
					if (scenario == null) {
						scenario = Scenario.LoadJson(File.ReadAllText(Path.Combine(folder, f)));
						nojson = false;
					}
					else
						scenario.AddJson(File.ReadAllText(Path.Combine(folder, f)));
				}
			}
			if(scenario == null)
			{
				if (nojson)
					DebugText.LogError("No scenario in folder: " + folder);
				else
					DebugText.LogError("Could not load scenario from folder: " + folder);
				UnloadScenario();
			}
		}
		catch (Exception e)
		{
			DebugText.LogError("Could not load scenario from folder: " +folder);
			Debug.LogException(e);
			UnloadScenario();
		}
		if(scenario == null)
		{
			UnloadScenario();
		}
		else if (scenario.scenes.Length == 0)
		{
			DebugText.LogError("No scenes found in folder: " +folder);
			UnloadScenario();
		}
		else
		{
			stateManager.EnterScenario();
			SwitchScene((Scene)null);
		}
	}

	void UnloadScenario()
	{
		scenario = null;
		currentScene = null;
		StopAllCoroutines();
		stateManager.EnterMenu();
		scenarioFolder = "";
	}

	void SwitchScene(string name)
	{
		if (name == "exit")
		{
			UnloadScenario();
			return;
		}
		else
		{
			Scene newscene = Array.Find<Scene>(scenario.scenes, (scene) => scene.name == name);
			if (newscene != null)
			{
				SwitchScene(newscene);
			}
			else
			{
				DebugText.LogError("Scene not defined: " + name);
				UnloadScenario();
				return;
			}
		}
	}

	void SwitchScene(Scene scene)
	{
		uiLayer.gameObject.SetActive(false);

		if(scene == null)
		{
			currentScene = new Scene();
			currentScene.background = "color:#333";
			currentScene.events = new aggrathon.vq360.data.Event[1];
			currentScene.events[0] = new aggrathon.vq360.data.Event();
			currentScene.events[0].time = 5f;
			currentScene.events[0].action = scenario.scenes[0].name;
		}
		else
		{
			currentScene = scene;
		}

		if(!File.Exists(Path.Combine(scenarioFolder, currentScene.background)))
		{
			Color c = Color.black;
			ColorUtility.TryParseHtmlString(currentScene.background.Substring(6), out c);
			colorLayer.SetColor(c, sceneChangeSpeed);
		}
		//TODO: Handle image
		//TODO: Handle video

		StopAllCoroutines();
		for (int i = 0; i < currentScene.events.Length; i++)
		{
			StartCoroutine(HandleEvent(currentScene.events[i]));
		}
	}

	IEnumerator HandleEvent(aggrathon.vq360.data.Event currentEvent)
	{
		yield return new WaitForSeconds(currentEvent.time);
		if(currentEvent.action != "")
		{
			HandleAction(currentEvent.action);
		}
		else
		{
			uiLayer.ShowQuestion(currentEvent, scenarioFolder, HandleAction);
		}
	}

	void HandleAction(string action)
	{
		if (action == "exit")
		{
			UnloadScenario();
		}
		else
		{
			SwitchScene(action);
		}
	}
}
