using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObserverConnectUI : MonoBehaviour {

	public ObserverManager manager;
	public GameObject roleSelection;
	public GameObject serverConnect;
	public GameObject clientConnect;

	[Header("Devices")]
	public GameObject buttonPrefab;
	public Transform deviceList;


	private void Start()
	{
		if (manager == null)
			manager = GameObject.FindObjectOfType<ObserverManager>();
		roleSelection.SetActive(false);
		serverConnect.SetActive(false);
		clientConnect.SetActive(false);
	}


	public bool Close()
	{
		if (roleSelection.activeSelf || serverConnect.activeSelf || clientConnect.activeSelf)
		{
			roleSelection.SetActive(false);
			serverConnect.SetActive(false);
			clientConnect.SetActive(false);
			return true;
		}
		return false;
	}


	public void Cancel()
	{
		manager.Disconnect();
		Close();
	}

	
	public void Setup()
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		manager.EnableBluetooth(() => { roleSelection.SetActive(true); });
#else
		DebugText.LogImportant("Bluetooth only available on Android");
#endif
	}

	public void Server()
	{
		manager.WaitForConnection((b) => {
			Close();
		});
	}

	public void Client()
	{
		string[] devices = manager.GetDevices();
		for (int i = 0; i < devices.Length - deviceList.childCount; i++)
		{
			Instantiate(buttonPrefab, deviceList);
		}
		for (int i = devices.Length; i < deviceList.childCount; i++)
		{
			deviceList.GetChild(i).gameObject.SetActive(false);
		}
		for (int i = 0; i < devices.Length; i++)
		{
			string device = devices[i];
			Transform button = deviceList.GetChild(i);
			button.GetComponent<Button>().onClick.RemoveAllListeners();
			button.GetComponent<Button>().onClick.AddListener(() => { Close(); manager.Connect(device); });
			button.GetChild(0).GetComponent<Text>().text = device;
			button.gameObject.SetActive(true);
		}
	}
}
