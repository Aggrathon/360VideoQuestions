using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

	public FadingText text;
	

	void Start () {
	}

	private void Update()
	{
		switch (state)
		{
			case State.btserver:
				if (bluetooth == null)
				{
					Disconnect();
				}
				else
				{
					string message = bluetooth.Call<string>("GetMessage");
					if(!string.IsNullOrEmpty(message))
					{
						if(message == "connected")
						{
							//TODO: Set up Broadcast
							Send("Test Message");
							if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(true);
						}
						else
						{
							DebugText.LogImportant(message);
							if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(false);
							Disconnect();
						}
					}
				}
				break;

			case State.broadcaster:
				break;

			case State.observer:
				string msg = bluetooth.Call<string>("GetMessage");
				if (!string.IsNullOrEmpty(msg))
				{
					text.Show(msg);
				}
				break;
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
			//TODO Set Observer Mode
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
			bluetooth.Call("Close");
			bluetooth = null;
		}
	}

	public void Disconnect()
	{
		OnDestroy();
		//TODO: Unload Observer
		state = State.idle;
	}

	#endregion

}
