using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class ObserverManager : MonoBehaviour {

    ConcurrentQueue<string> messageQueue;
    AndroidJavaObject bluetooth;
    Thread listenerThread;
	Action bluetoothEnabledCallback;
	Action<bool> bluetoothConnectedCallback;

	public FadingText text;
	

	void Start () {
        messageQueue = new ConcurrentQueue<string>();
	}

	private void OnDestroy()
	{
		if (bluetooth != null)
		{
			bluetooth.Call("Close");
			bluetooth = null;
		}
		if (listenerThread != null)
		{
			listenerThread.Abort();
			listenerThread = null;
		}
	}

	private void Update()
	{
		if (messageQueue.Count > 0)
		{
			string str;
			if (messageQueue.TryDequeue(out str))
				text.Show(str);
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
		listenerThread = new Thread(Server);
		listenerThread.Start();
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
			listenerThread = new Thread(Listen);
			listenerThread.Start();
		}
		else
		{
			Disconnect();
			DebugText.LogImportant("Could not connect to "+device);
		}
	}

	public void Disconnect()
	{
		OnDestroy();
		if (!messageQueue.IsEmpty)
			messageQueue = new ConcurrentQueue<string>();
		//TODO Unload Observer
	}

	#endregion


	#region Threads

	private void Listen()
    {
        while(bluetooth != null)
        {
            messageQueue.Enqueue(bluetooth.Call<string>("Listen"));
        }
    }

	private void Server()
	{
		if (bluetooth.Call<bool>("ConnectServer"))
		{
			//TODO Start broadcasting
			if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(true);
		}
		else
		{
			Disconnect();
			if (bluetoothConnectedCallback != null) bluetoothConnectedCallback(false);
		}
	}

	#endregion

}
