using UnityEngine;

public struct MessageData
{
	public string scenario;
	public string scene;
	public Vector3 rotation;
	public int permutation;

	public MessageData(string msg)
	{
		int end = msg.Length;
		int start = msg.LastIndexOf(',');
		permutation = int.Parse(msg.Substring(start + 1, end-1-start));
		end = start;
		start = msg.LastIndexOf(',', end - 1);
		float z = float.Parse(msg.Substring(start + 1, end - 1 - start));
		end = start;
		start = msg.LastIndexOf(',', end - 1);
		float y = float.Parse(msg.Substring(start + 1, end - 1 - start));
		end = start;
		start = msg.LastIndexOf(',', end - 1);
		float x = float.Parse(msg.Substring(start + 1, end - 1 - start));
		rotation = new Vector3(x, y, z);
		end = start;
		start = msg.IndexOf(',');
		scenario = msg.Substring(0, start);
		scene = msg.Substring(start+1, end - 1 - start);
	}

	public MessageData(string scenario, string scene, Vector3 rot, int perm)
	{
		this.scenario = scenario;
		this.scene = scene;
		rotation = rot;
		permutation = perm;
	}

	public MessageData(ObserverManager om)
	{
#if UNITY_EDITOR
		rotation = om.mainCamera.transform.eulerAngles;
#else
			rotation = InputTracking.GetLocalRotation(VRNode.Head).eulerAngles;
#endif
		if (om.scenarioManager.gameObject.activeSelf)
		{
			scenario = om.scenarioManager.GetScenarioName();
			scene = om.scenarioManager.GetSceneName();
			permutation = om.scenarioManager.GetPermutationNumber();
		}
		else
		{
			scenario = "";
			scene = "";
			permutation = 0;
		}
	}

	override public string ToString()
	{
		return string.Format("{0},{1},{2},{3},{4},{5}", scenario, scene, rotation.x, rotation.y, rotation.z, permutation);
	}
}