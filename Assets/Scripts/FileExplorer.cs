using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VR;

public class FileExplorer : MonoBehaviour {

	const string FOLDER_NAME = "360VideoQuestions";

	public Text[] insertFolderPathFormatted;

	public string dataFolder { get; protected set; }
	public GameObject scenarioButton;
	public Transform scenarioButtonHolder;

	void OnEnable() {
		VRSettings.enabled = false;
		Screen.orientation = ScreenOrientation.Portrait;
	}

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

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			Application.Quit();
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
				DebugText.Log("Android Enviroment Documents: " + root);
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
			//button.GetComponent<Button>().onClick.AddListener()
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
		if(!Directory.CreateDirectory(Path.Combine(dataFolder, "Example")).Exists)
		{
			DebugText.LogError("Could not create Example folder");
		}
		RefreshFolder();
	}
}
