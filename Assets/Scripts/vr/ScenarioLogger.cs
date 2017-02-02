using System;
using System.IO;
using System.Text;
using UnityEngine;

public class ScenarioLogger : MonoBehaviour {

	public float logInterval = 0.2f;

	StreamWriter writer;
	Transform mainCamera;
	float startTime;
	float sceneTime;
	string currentScene;
	float intervalTime;

	void Start()
	{
		mainCamera = Camera.main.transform;
	}

	public void StartLogging(string folder)
	{
		startTime = Time.time;
		currentScene = "unkown";
		sceneTime = startTime;
		intervalTime = 0f;
		try
		{
			string foldername = Path.Combine(folder, "logs");
			Directory.CreateDirectory(foldername);
			string filename = Path.Combine(foldername, DateTime.Now.ToString("yyyyMMddHHmmss")+".csv");
			writer = new StreamWriter(filename, false, Encoding.UTF8);
			writer.WriteLine("\"Total Time\",\"Scene Time\",\"Scene\",\"Yaw\",\"Pitch\",\"Roll\",\"Extra Information\"");
			enabled = true;
		}
		catch (Exception e)
		{
			if(writer != null)
			{
				writer.Close();
				writer = null;
			}
			DebugText.LogException("Could not create logfile", e);
			enabled = false;
		}
	}

	public void SwitchScene(string name)
	{
		currentScene = name;
		sceneTime = Time.time;
	}
	
	void Update () {
		if (writer == null)
		{
			enabled = false;
			return;
		}
		intervalTime -= Time.deltaTime;
		if(intervalTime <= 0f)
		{
			intervalTime += logInterval;
			try
			{
				Vector3 rot = mainCamera.eulerAngles;
				writer.WriteLine(string.Format("{0:F2},{1:F2},\"{2}\",{3:F2},{4:F2},{5:F2},\"{6}\"",
					Time.time - startTime,
					Time.time - sceneTime,
					currentScene,
					rot.y,
					rot.x,
					rot.z,
					""
					));
			}
			catch (Exception e)
			{
				if (writer != null)
				{
					writer.Close();
					writer = null;
				}
				DebugText.LogException("Could not write to logfile", e);
				enabled = false;
				return;
			}
		}
	}

	private void OnDisable()
	{
		if(writer != null)
		{
			try
			{
				writer.Close();
			}
			catch (Exception e)
			{
				DebugText.LogException("Could not close logfile", e);
			}
			writer = null;
		}
	}
}
