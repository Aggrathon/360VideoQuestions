using System.IO;
using UnityEngine;
using UnityEngine.VR;

public class FileExplorer : MonoBehaviour {

	const string FOLDER_NAME = "360VideoQuestions";

	void Start () {
		VRSettings.enabled = false;
		Screen.orientation = ScreenOrientation.Portrait;
		DebugText.Log("Unity External: " + Application.persistentDataPath);

#if UNITY_ANDROID && !UNITY_EDITOR
		using (AndroidJavaClass jc = new AndroidJavaClass("android.os.Environment"))
		{
			string docs = jc.GetStatic<string>("DIRECTORY_DOCUMENTS");
			string root = jc.CallStatic<AndroidJavaObject>("getExternalStoragePublicDirectory", docs).Call<string>("getPath");
			DebugText.Log("Android Enviroment Documents: " + root);
			try
			{
				string folder = Path.Combine(root, FOLDER_NAME);
				if (Directory.CreateDirectory(folder).Exists)
				{
					DebugText.Log("Data folder created at: " + folder);
				}
			}
			catch (System.Exception e)
			{
				DebugText.Log("<color=red>Could not create directory</color>");
			}
		}
#endif
	}

	void Update()
	{
		if (Input.GetKeyUp(KeyCode.Escape))
			Application.Quit();
	}
}
