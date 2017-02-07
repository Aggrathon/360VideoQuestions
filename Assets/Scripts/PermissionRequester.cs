using UnityEngine;
using System;

public class PermissionRequester : MonoBehaviour
{
	private static PermissionRequester instance;

	private	Action<bool> permissionCallback;

	public static void RequestPermisssion(Action<bool> callback)
	{
#if UNITY_ANDROID && !UNITY_EDITOR
		try
		{
			using (AndroidJavaClass prc = new AndroidJavaClass("aggrathon.vq360.javaplugin.PermissionRequester"))
			{
				AndroidJavaClass uac = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidJavaObject activity = uac.GetStatic<AndroidJavaObject>("currentActivity");
				if (prc.CallStatic<bool>("HasExternalPermission", activity))
				{
					if (callback != null) callback(true);
					return;
				}
				else
				{
					if (instance == null)
					{
						GameObject go = new GameObject("Permission Requester");
						instance = go.AddComponent<PermissionRequester>();
						DontDestroyOnLoad(go);
					}
					instance.permissionCallback = callback;
					prc.CallStatic("GetExternalPermission", activity, instance.name, "PermissionRequestCallback");
				}
			}
		}
		catch (Exception e)
		{
			DebugText.LogException("Could not request permissions", e);
			if (callback != null) callback(false);
		}
#else
		if (callback != null) callback(true);
#endif
	}

	private void PermissionRequestCallback(string message)
	{
		if (permissionCallback != null)
		{
			StartCoroutine(Utils.RunLater(() => {
				permissionCallback(message == "true");
				permissionCallback = null;
			}, (YieldInstruction)null));
		}
	}
}
