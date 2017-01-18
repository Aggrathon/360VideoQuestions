using UnityEngine;
using UnityEngine.VR;

public class AppStateManager : MonoBehaviour {

	public enum AppState
	{
		menu,
		scenario
	}

	public AppState state;
	public GameObject menu;
	public GameObject scenario;
	public InformationPanel menuInfo;

	void Start()
	{
		switch (state)
		{
			case AppState.menu:
				EnterMenu();
				break;
			case AppState.scenario:
				EnterScenario();
				break;
		}
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

	void EnterMenu()
	{
		VRSettings.enabled = false;
		Screen.orientation = ScreenOrientation.Portrait;
		state = AppState.menu;
		menu.SetActive(true);
		scenario.SetActive(false);
	}

	void EnterScenario()
	{
		VRSettings.enabled = true;
		Screen.orientation = ScreenOrientation.Landscape;
		state = AppState.scenario;
		menu.SetActive(false);
		scenario.SetActive(true);
	}
}
