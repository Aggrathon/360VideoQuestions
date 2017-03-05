using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Video;
using aggrathon.vq360.data;

[RequireComponent(typeof(ScenarioLogger))]
public class ScenarioManager : MonoBehaviour {

	public AppStateManager stateManager;
	public VideoLayer videoLayer;
	public MeshRenderer photoLayer;
	public ColorLayer colorLayer;
	public UILayer uiLayer;
	public float sceneChangeSpeed = 0.5f;

	Scenario scenario;
	Scene currentScene;
	string scenarioFolder;
	ScenarioLogger logger;
	int startScene = 0;

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
			if (logger == null)
				logger = GetComponent<ScenarioLogger>();
			scenario.InitScenes();
			startScene = 0;
			logger.StartLogging(scenarioFolder);
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
		StopAllCoroutines();
		bool video = false;

		if (scene == null)
		{
			currentScene = new Scene();
			currentScene.background = "#333";
			currentScene.events = new aggrathon.vq360.data.Event[1];
			currentScene.events[0] = new aggrathon.vq360.data.Event();
			currentScene.events[0].time = 5f;
			currentScene.events[0].action = "exit";
		}
		else
		{
			currentScene = scene;
		}

		if (currentScene.background != "")
		{
			if (!File.Exists(Path.Combine(scenarioFolder, currentScene.background)))
			{
				Color c = Color.black;
				ColorUtility.TryParseHtmlString(currentScene.background, out c);
				colorLayer.SetColor(c, colorLayer.gameObject.activeSelf? sceneChangeSpeed *2f : sceneChangeSpeed);

				if (videoLayer.gameObject.activeSelf)
					videoLayer.Hide(sceneChangeSpeed);

				StartCoroutine(Utils.RunLater(() => {
					if (photoLayer.gameObject.activeSelf)
						photoLayer.gameObject.SetActive(false);
					logger.SwitchScene(currentScene.name);
				}, sceneChangeSpeed * 0.5f));
			}
			else
			{
				string ext = Path.GetExtension(currentScene.background).ToLower();
				if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
				{
					photoLayer.material.mainTexture = Utils.LoadImage(Path.Combine(scenarioFolder, currentScene.background));

					colorLayer.Flash(sceneChangeSpeed);
					StartCoroutine(Utils.RunLater(() => {
						if (!photoLayer.gameObject.activeSelf)
							photoLayer.gameObject.SetActive(true);
						logger.SwitchScene(currentScene.name);
					}, sceneChangeSpeed * 0.5f));
					if (videoLayer.gameObject.activeSelf)
						videoLayer.Hide(sceneChangeSpeed*0.5f);
				}
				else if (ext == ".mp4")
				{
					colorLayer.TurnOn(sceneChangeSpeed*0.5f);
					videoLayer.SetVideo(Path.Combine(scenarioFolder, currentScene.background), currentScene.ending, HandleAction, sceneChangeSpeed*0.5f, ()=> {
						colorLayer.TurnOff(sceneChangeSpeed * 0.5f);
						logger.SwitchScene(currentScene.name);

						for (int i = 0; i < currentScene.events.Length; i++)
						{
							StartCoroutine(HandleEvent(currentScene.events[i], 0));
						}
					});

					video = true;
					if (photoLayer.gameObject.activeSelf)
						StartCoroutine(Utils.RunLater(() => photoLayer.gameObject.SetActive(false), sceneChangeSpeed*0.5f));
				}
				else
				{
					DebugText.LogImportant("Unsupported background file format in scene '" + currentScene.name + "'");
					StartCoroutine(Utils.RunLater(() => logger.SwitchScene(currentScene.name), sceneChangeSpeed * 0.5f));
					videoLayer.ClearAction();
				}
			}
		}
		else
		{
			StartCoroutine(Utils.RunLater(() => logger.SwitchScene(currentScene.name), sceneChangeSpeed * 0.5f));
			videoLayer.ClearAction();
		}

		if(!video) for (int i = 0; i < currentScene.events.Length; i++)
		{
			StartCoroutine(HandleEvent(currentScene.events[i], 0.5f * sceneChangeSpeed));
		}
	}

	IEnumerator HandleEvent(aggrathon.vq360.data.Event currentEvent, float delay)
	{
		yield return new WaitForSeconds(currentEvent.time+delay);
		if(currentEvent.action != "")
		{
			HandleAction(currentEvent.action);
		}
		else
		{
			currentEvent.ScrambleOptions();
			logger.LogInformation(currentEvent.GetOptionText());
			uiLayer.ShowQuestion(currentEvent, scenarioFolder, HandleOption);
		}
	}

	void HandleAction(string action)
	{
		if (action == "exit")
		{
			if (startScene < scenario.start.Length)
			{
				action = scenario.start[startScene];
				startScene++;
				HandleAction(action);
			}
			else
			{
				UnloadScenario();
			}
		}
		else
		{
			SwitchScene(action);
		}
	}

	void HandleOption(Option option)
	{
		logger.LogInformation("Selected: " + option.GetName());
		HandleAction(option.action);
	}
}
