package aggrathon.vq360.javaplugin;

import android.Manifest;
import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.pm.PackageManager;
import android.os.Build;
import android.support.v13.app.FragmentCompat;

import com.unity3d.player.UnityPlayer;

public class PermissionRequester extends Fragment {

	private static final int REQUEST_WRITE_EXTERNAL = 994;
	private String unityCallbackName;
	private String unityCallbackMethod;
	private FragmentManager fManager;


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

		try {
			PermissionRequester frag = new PermissionRequester();
			frag.fManager = activity.getFragmentManager();
			frag.unityCallbackName = unityObjectName;
			frag.unityCallbackMethod = unityMethodName;

			FragmentTransaction ft = frag.fManager.beginTransaction();
			ft.add(0, frag);
			ft.commit();
		}
		catch (Exception e) {
			UnityPlayer.UnitySendMessage(unityObjectName, unityMethodName, "");
		}
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
				UnityPlayer.UnitySendMessage(unityCallbackName, unityCallbackMethod, "true");
			}
			else {
				UnityPlayer.UnitySendMessage(unityCallbackName, unityCallbackMethod, "");
			}
		}
		else {
			super.onRequestPermissionsResult(requestCode, permissions, grantResults);
		}
		FragmentTransaction ft = fManager.beginTransaction();
		ft.remove(this);
		ft.commit();
	}

}
