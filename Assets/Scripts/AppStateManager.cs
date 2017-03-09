using UnityEngine;
using UnityEngine.VR;
using aggrathon.vq360.data;

public class AppStateManager : MonoBehaviour {

	public enum AppState
	{
		menu,
		scenario
	}

	[SerializeField] GameObject menu;
	[SerializeField] ScenarioManager scenario;
	[SerializeField] PopupPanel menuInfo;

	AppState state;


	void Start()
	{
		EnterMenu();
	}

	void Update()
	{
		switch (state)
		{
			case AppState.menu:
				if(Input.GetKeyUp(KeyCode.Escape) && !menuInfo.Close())
				{
					Application.Quit();
				}
				else  if (Input.GetKeyUp(KeyCode.Menu))
				{
					menuInfo.Toggle();
				}
				break;
			case AppState.scenario:
				if (Input.GetKeyUp(KeyCode.Escape))
				{
					EnterMenu();
				}
				break;
		}
	}

	public void EnterMenu()
	{
		VRSettings.enabled = false;
		Screen.orientation = ScreenOrientation.Portrait;
		state = AppState.menu;
		menu.SetActive(true);
		scenario.gameObject.SetActive(false);
	}

	public void EnterScenario()
	{
		VRSettings.enabled = true;
		Screen.orientation = ScreenOrientation.Landscape;
		state = AppState.scenario;
		menu.SetActive(false);
		scenario.gameObject.SetActive(true);
	}

#if !UNITY_EDITOR
	private void OnApplicationFocus(bool focus)
	{
		EnterMenu();
	}
#endif

}
