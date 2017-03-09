using System;
using System.Collections.Concurrent;
using System.Threading;
using UnityEngine;

public class ObserverManager : MonoBehaviour {

    ConcurrentQueue<string> messageQueue;
    AndroidJavaObject bluetooth;
    Thread listenerThread;
	Action<bool> bluetoothEnabledCallback;
	Action<bool> bluetoothConnectedCallback;
	

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
			//Parse Messages
		}
	}


	#region Bluetooth Settings

	public void EnableBluetooth(Action<bool> onEnable)
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
			if (bluetoothEnabledCallback != null) bluetoothEnabledCallback(true);
		}
		else
		{
			DebugText.LogImportant("Bluetooth needed for observing");
			if (bluetoothEnabledCallback != null) bluetoothEnabledCallback(false);
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
		if (bluetooth.Call<bool>("ConnectClient"))
		{
			//TODO Set Observer Mode
			listenerThread = new Thread(Listen);
			listenerThread.Start();
		}
		else
		{
			Disconnect();
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
