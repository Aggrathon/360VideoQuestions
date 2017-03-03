using UnityEngine;
using UnityEngine.VR;

public class EditorCameraMovement : MonoBehaviour {

#if UNITY_EDITOR
	void Update () {
		if(Input.GetKey(KeyCode.LeftAlt))
		{
			Vector3 rot = transform.eulerAngles;
			rot.x -= Input.GetAxis("Mouse Y");
			rot.y += Input.GetAxis("Mouse X");
			transform.eulerAngles = rot;
		}
	}
#endif

}
