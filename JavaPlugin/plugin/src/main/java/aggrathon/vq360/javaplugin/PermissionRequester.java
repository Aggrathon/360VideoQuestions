package aggrathon.vq360.javaplugin;

import android.Manifest;
import android.app.Activity;
import android.content.pm.PackageManager;
import android.os.Build;
import android.support.v13.app.FragmentCompat;

import com.unity3d.player.UnityPlayer;

public class PermissionRequester extends UnityFragment {

	private static final int REQUEST_WRITE_EXTERNAL = 994;

	public static boolean HasExternalPermission(Activity activity) {
		if (Build.VERSION.SDK_INT < 23)
			return true;

		return PackageManager.PERMISSION_GRANTED == activity.checkCallingOrSelfPermission(Manifest.permission.WRITE_EXTERNAL_STORAGE);
	}

	public static void GetExternalPermission(Activity activity, String unityObjectName, String unityMethodName) {
		if (HasExternalPermission(activity)){
			UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "true");
			return;
		}
		CallFragment(new PermissionRequester(), activity, unityObjectName, unityMethodName);
	}

	@Override
	public void onStart() {
		super.onStart();
		FragmentCompat.requestPermissions(this, new String[] {Manifest.permission.WRITE_EXTERNAL_STORAGE}, REQUEST_WRITE_EXTERNAL);
	}

	@Override
	public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
		if (requestCode == REQUEST_WRITE_EXTERNAL) {
			if(grantResults[0] == PackageManager.PERMISSION_GRANTED) {
				Callback("true");
			}
			else {
				Callback("false");
			}
			CleanUp();
		}
		else {
			super.onRequestPermissionsResult(requestCode, permissions, grantResults);
		}
	}

}
