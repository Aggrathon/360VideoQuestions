package aggrathon.vq360.javaplugin;


import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;

public class BluetoothEnabler extends UnityFragment {
	
	private static final int REQUEST_BLUETOOTH_ON = 226;

	public static void SetEnabled(Activity activity, String unityObject, String unityMethod) {
		CallFragment(new BluetoothEnabler(), activity, unityObject, unityMethod);
	}

	@Override
	public void onStart() {
		super.onStart();
		BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		if (mBluetoothAdapter == null) {
			Callback("Device does not support Bluetooth");
			CleanUp();
		}
		else if (!mBluetoothAdapter.isEnabled()) {
			Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
			startActivityForResult(enableBtIntent, REQUEST_BLUETOOTH_ON);
		}
		else {
			Callback("true");
			CleanUp();
		}
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if(requestCode == REQUEST_BLUETOOTH_ON) {
			if (resultCode == Activity.RESULT_OK) {
				Callback("true");
			}
			else {
				Callback("false");
			}
			CleanUp();
		}
		else
			super.onActivityResult(requestCode, resultCode, data);
	}
}
