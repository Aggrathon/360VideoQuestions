using System;
using System.Collections;
using System.IO;
using UnityEngine;

public class ScenarioManager : MonoBehaviour {

	public AppStateManager stateManager;
	public ColorLayer colorLayer;
	public float sceneChangeSpeed = 0.5f;

	RuntimeScenario scenario;
	Scene currentScene;

	public void LoadScenario(string folder)
	{
		scenario = null;
		try {
			var files = Directory.GetFiles(folder);
			bool nojson = true;
			foreach (string f in files)
			{
				if (Path.GetExtension(f) == "json")
				{
					if (scenario == null) {
						scenario = RuntimeScenario.LoadJson(File.ReadAllText(Path.Combine(folder, f)));
						nojson = false;
					}
					else
						scenario.AddJson(File.ReadAllText(Path.Combine(folder, f)));
				}
			}
			if(scenario == null)
			{
				if (nojson)
					DebugText.LogError("No scenario in folder: " + Path.GetDirectoryName(folder));
				else
					DebugText.LogError("Could not load scenario from folder: " + Path.GetDirectoryName(folder));
				UnloadScenario();
			}
		}
		catch (Exception e)
		{
			DebugText.LogError("Could not load scenario from folder: " + Path.GetDirectoryName(folder));
			Debug.LogException(e);
			UnloadScenario();
		}
		if(scenario == null)
		{
			UnloadScenario();
		}
		else if (scenario.scenes.Length == 0)
		{
			DebugText.LogError("No scenes found in folder: " + Path.GetDirectoryName(folder));
			UnloadScenario();
		}
		else
		{
			stateManager.EnterScenario();
			SwitchScene((Scene)null);
		}
	}

	public void LoadScenario(RuntimeScenario rs)
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
			currentScene.actions = new Action[1];
			currentScene.actions[0] = new Action();
			currentScene.actions[0].time = 5f;
			currentScene.actions[0].action = scenario.scenes[0].name;
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
		for (int i = 0; i < currentScene.actions.Length; i++)
		{
			StartCoroutine(HandleAction(currentScene.actions[i]));
		}
	}

	IEnumerator HandleAction(Action action)
	{
		yield return new WaitForSeconds(action.time);
		if(action.action != "")
		{
			if(action.action == "exit")
			{
				UnloadScenario();
			}
			else
			{
				SwitchScene(action.action);
			}
		}
		else
		{
			//TODO: Handle questions
		}
	}
}
