using UnityEngine;
using UnityEngine.VR;

public class CameraDebug : MonoBehaviour {
	
	void Update () {
		if(Input.GetKey(KeyCode.LeftAlt))
		{
			Vector3 rot = transform.parent.eulerAngles;
			rot.x -= Input.GetAxis("Mouse Y");
			rot.y += Input.GetAxis("Mouse X");
			transform.parent.eulerAngles = rot;
		}
	}
}
