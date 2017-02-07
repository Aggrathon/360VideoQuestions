package aggrathon.vq360.javaplugin;

import android.Manifest;
import android.content.pm.PackageManager;
import android.support.v4.app.ActivityCompat;
import android.support.v4.content.ContextCompat;

import com.unity3d.player.UnityPlayer;

public class JavaActivity extends UnityPlayerActivity {

	private static final int REQUEST_WRITE_EXTERNAL = 994;
	private String unityCallbackName;
	private String unityCallbackMethod;


	public boolean HasExternalPermission() {
		return PackageManager.PERMISSION_GRANTED == ContextCompat.checkSelfPermission(this, Manifest.permission.WRITE_EXTERNAL_STORAGE);
	}

	public void GetExternalPermission(String unityObjectName, String unityMethodName) {
		unityCallbackName = unityObjectName;
		unityCallbackMethod = unityMethodName;
		if (HasExternalPermission()){
			return;
		}

		ActivityCompat.requestPermissions(this, new String[] {Manifest.permission.WRITE_EXTERNAL_STORAGE}, REQUEST_WRITE_EXTERNAL);
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
	}

}
