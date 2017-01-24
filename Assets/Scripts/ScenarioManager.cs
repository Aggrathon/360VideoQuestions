using System;
using System.Collections;
using System.IO;
using UnityEngine;
using aggrathon.vq360.data;

public class ScenarioManager : MonoBehaviour {

	public AppStateManager stateManager;
	public ColorLayer colorLayer;
	public float sceneChangeSpeed = 0.5f;

	Scenario scenario;
	Scene currentScene;

	public void LoadScenario(string folder)
	{
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

	public void LoadScenario(Scenario rs)
	{
		scenario = rs;
		stateManager.EnterScenario();
		SwitchScene((Scene)null);
	}

	void UnloadScenario()
	{
		scenario = null;
		currentScene = null;
		StopAllCoroutines();
		stateManager.EnterMenu();
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

		if(currentScene.background.StartsWith("color:"))
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
			if(currentEvent.action == "exit")
			{
				UnloadScenario();
			}
			else
			{
				SwitchScene(currentEvent.action);
			}
		}
		else
		{
			//TODO: Handle questions
		}
	}
}
