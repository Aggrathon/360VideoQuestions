using System;
using System.IO;
using System.Security;
using UnityEngine;
using UnityEngine.UI;
using aggrathon.vq360.data;

public class FileExplorer : MonoBehaviour {

	const string FOLDER_NAME = "360VideoQuestions";

	public Text[] insertFolderPathFormatted;

	public string dataFolder { get; protected set; }
	public GameObject scenarioButton;
	public Transform scenarioButtonHolder;
	public ScenarioManager scenarioManager;
	public EditorScenario exampleScenario;

	void Start()
	{
		dataFolder = GetScenarioFolder();
		if (dataFolder == "")
		{
			DebugText.LogError("Could not read/create the folder for scenarios");
		}
		else
		{
			foreach (Text t in insertFolderPathFormatted)
			{
				t.text = string.Format(t.text, dataFolder);
			}
			RefreshFolder();
		}
	}

	string GetScenarioFolder()
	{
#if UNITY_EDITOR
		string path = Path.Combine(Path.Combine(Application.dataPath, "Editor"), "scenarios");
		if (Directory.CreateDirectory(path).Exists)
			return path;
#elif UNITY_ANDROID
		try
		{
			using (AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment"))
			{
				string docs = jc.GetStatic<string>("DIRECTORY_DOCUMENTS");
				string root = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", docs).Call<string>("getPath");
				string folder = Path.Combine(root, FOLDER_NAME);
				if (Directory.CreateDirectory(folder).Exists)
				{
					return folder;
				}
			}
		}
		catch (Exception e)
		{
			DebugText.Log(string.Format("<color=red>{0}</color>", e.Message));
		}
#endif
		return "";
	}

	public void RefreshFolder()
	{
		for (int i = scenarioButtonHolder.childCount - 1; i >= 0; i--)
		{
			Destroy(scenarioButtonHolder.GetChild(i).gameObject);
		}
		string[] dirs = Directory.GetDirectories(dataFolder);
		foreach (string dir in dirs)
		{
			GameObject button = GameObject.Instantiate<GameObject>(scenarioButton, scenarioButtonHolder, false);
			button.GetComponent<Button>().onClick.AddListener(() => {
				scenarioManager.LoadScenario(Path.Combine(dataFolder, dir));
			});
			button.GetComponentInChildren<Text>().text = Path.GetFileName(dir);
		}
	}

	public void OpenFolder()
	{
		if(dataFolder!="")
		{
#if UNITY_ANDROID && !UNITY_EDITOR
			using (AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent"))
			{
				AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
				AndroidJavaObject uri = uriClass.CallStatic<AndroidJavaObject>("parse", "File://" + dataFolder);
				AndroidJavaClass activityClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = activityClass.GetStatic<AndroidJavaObject>("currentActivity");
				try
				{
					AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_VIEW"));
					intent.Call<AndroidJavaObject>("setDataAndType", uri, "resource/folder");
					activity.Call("startActivity", intent);
				}
				catch (Exception e)
				{
					AndroidJavaObject intent = new AndroidJavaObject("android.content.Intent", intentClass.GetStatic<string>("ACTION_VIEW"));
					intent.Call<AndroidJavaObject>("setDataAndType", uri, "*/*");
					activity.Call("startActivity", intentClass.CallStatic<AndroidJavaObject>("createChooser", intent, "Open folder"));
				}
			}
#else
			Application.OpenURL("File://" + dataFolder);
#endif
		}
	}

	public void CreateExample()
	{
		if (dataFolder == "")
			return;
		string path = Path.Combine(dataFolder, "Example");
		if (!Directory.CreateDirectory(path).Exists)
		{
			DebugText.LogError("Could not create Example folder");
		}
		else
		{
			try
			{
				System.IO.File.WriteAllText(Path.Combine(path, "scenes.json"), exampleScenario.ToJson());
				System.IO.File.WriteAllText(Path.Combine(path, "instructions.txt"), exampleScenario.instructions);
				for (int i = 0; i < exampleScenario.images.Length; i++)
				{
					if (exampleScenario.images[i].name != "" && exampleScenario.images[i].image != null)
					{
						SaveImage(exampleScenario.images[i].image, Path.Combine(path, exampleScenario.images[i].name));
					}
				}
			}
			catch (Exception e)
			{
				DebugText.LogError("Could not save example scenario (" + e.GetType().ToString() + ")");
			}
		}
		RefreshFolder();
		DebugText.Log("Created Example Scenario");
	}

	public static void SaveImage(Texture2D image, string path)
	{
		try
		{
			System.IO.File.WriteAllBytes(path, image.EncodeToPNG());
		}
		catch (Exception e)
		{
			if (e is SecurityException || e is UnauthorizedAccessException)
			{
				DebugText.LogError("App not allowed to save image to " + path);
			}
			else
			{
				DebugText.LogError("Could not save image to " + path + " (" + e.GetType().ToString() + ")");
			}
		}
	}
}
