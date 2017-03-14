using System;
using UnityEngine;
using UnityEngine.VR;

public class ObserverManager : MonoBehaviour {

	public enum State
	{
		idle,
		btserver,
		broadcaster,
		observer
	}
	
	AndroidJavaObject bluetooth;
	Action bluetoothEnabledCallback;
	Action<bool> bluetoothConnectedCallback;
	State state = State.idle;
	float time;
	internal Camera mainCamera;
	MessageData data;

	public float sendInterval = 0.2f;
	public FadingText text;
	public ScenarioManager scenarioManager;
	public AppStateManager appState;
	

	void Start () {
		mainCamera = Camera.main;
		data = new MessageData();
	}

	private void Update()
	{
		if(state != State.idle && bluetooth == null)
		{
			Disconnect();
			return;
		}
		switch (state)
		{
			case State.btserver:
				string message = bluetooth.Call<string>("GetMessage");
				if(!string.IsNullOrEmpty(message))
				{
					if(message == "connected")
					{
						state = State.broadcaster;
						if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(true);
					}
					else
					{
						DebugText.LogImportant(message);
						if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(false);
						Disconnect();
					}
				}
				break;

			case State.broadcaster:
				time -= Time.deltaTime;
				if (time < 0)
				{
					time += sendInterval;
					if (scenarioManager.gameObject.activeSelf)
					{
						data = new MessageData(this);
						Send(data.ToString());
					}
					else
					{
						Send("idle");
					}
				}
				break;

			case State.observer:
				string msg = bluetooth.Call<string>("GetMessage");
				if (!string.IsNullOrEmpty(msg))
				{
					HandleMessage(msg);
				}
				break;
		}
	}

	public void HandleMessage(string msg)
	{
		if(msg == "idle")
		{
			text.Show("Waiting for scenario selection");
			scenarioManager.gameObject.SetActive(false);
		}
		else if(msg == "disconnect")
		{
			Disconnect();
		}
		else
		{
			try
			{
				MessageData ndata = new MessageData(msg);
				if(!scenarioManager.gameObject.activeSelf || ndata.scenario != data.scenario)
				{
					scenarioManager.LoadScenario(ndata.scenario, true);
				}
				if(data.scene != ndata.scene)
				{
					scenarioManager.SwitchScene(ndata.scene == "" ? null : ndata.scene);
					if (data.permutation != ndata.permutation)
						scenarioManager.SetPermutationNumber(ndata.permutation);
				}
				if (string.IsNullOrEmpty(ndata.scene))
				{
					text.Hide();
				}
				mainCamera.transform.rotation = Quaternion.Euler(ndata.rotation);

				data = ndata;
			}
			catch (Exception)
			{
				data = new MessageData(this);
			}
			//TODO: Remove
			text.Show(msg);
		}
	}

	public void Send(string message)
	{
		if(bluetooth != null)
		{
			bluetooth.Call("Send", message);
		}
	}


	#region Bluetooth Settings

	public void EnableBluetooth(Action onEnable)
    {
		bluetoothEnabledCallback = onEnable;
		using (AndroidJavaClass cls = new AndroidJavaClass("aggrathon.vq360.javaplugin.BluetoothSettings"))
		{
			AndroidJavaClass uac = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = uac.GetStatic<AndroidJavaObject>("currentActivity");
			cls.CallStatic("SetEnabled", activity, gameObject.name, "EnableBluetoothCallback");
		}
    }

	public void OpenBluetoothSettings()
	{
		using (AndroidJavaClass cls = new AndroidJavaClass("aggrathon.vq360.javaplugin.BluetoothSettings"))
		{
			AndroidJavaClass uac = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			AndroidJavaObject activity = uac.GetStatic<AndroidJavaObject>("currentActivity");
			cls.CallStatic("OpenSettings", activity);
		}
	}

    public void EnableBluetoothCallback(string result)
    {
		if (result == "true")
		{
			if (bluetoothEnabledCallback != null) bluetoothEnabledCallback();
		}
		else
		{
			DebugText.LogImportant("Bluetooth needed for observing");
		}
    }

	#endregion


	#region Connect

	public void WaitForConnection(Action<bool> onResult)
    {
		bluetoothConnectedCallback = onResult;
		state = State.btserver;
		if(bluetooth == null)
			bluetooth = new AndroidJavaObject("aggrathon.vq360.javaplugin.Bluetooth");
		bluetooth.Call("ConnectServer");
	}

    public string[] GetDevices()
    {
        bluetooth = new AndroidJavaObject("aggrathon.vq360.javaplugin.Bluetooth");
        return bluetooth.Call<string>("GetDevices").Split(',');
	}

	public void Connect(string device)
	{
		if (bluetooth == null)
			GetDevices();
		if (bluetooth.Call<bool>("ConnectClient", device))
		{
			appState.EnterObserver();
			state = State.observer;
		}
		else
		{
			Disconnect();
			DebugText.LogImportant("Could not connect to "+device);
		}
	}

	private void OnDestroy()
	{
		if (bluetooth != null)
		{
			if (state == State.broadcaster)
			{
				Send("disconnect");
			}
			bluetooth.Call("Close");
			bluetooth = null;
		}
	}

	public void Disconnect()
	{
		OnDestroy();
		appState.EnterMenu();
		state = State.idle;
		StopAllCoroutines();
	}

	#endregion

}
