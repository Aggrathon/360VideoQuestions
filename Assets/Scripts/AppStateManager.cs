using UnityEngine;
using UnityEngine.VR;

public class AppStateManager : MonoBehaviour {

	public enum AppState
	{
		menu,
		scenario
	}

	[SerializeField] AppState state;
	[SerializeField] GameObject menu;
	[SerializeField] ScenarioManager scenario;
	[SerializeField] InformationPanel menuInfo;
	public Scenario defScen;

	void Start()
	{
		switch (state)
		{
			case AppState.menu:
				EnterMenu();
				break;
			case AppState.scenario:
				scenario.LoadScenario((RuntimeScenario)defScen);
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
}
