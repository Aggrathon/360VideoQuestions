package aggrathon.vq360.javaplugin;

import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;

public class BluetoothDiscoverer extends UnityFragment {


	private static final int REQUEST_BLUETOOTH_DISCOVER = 4355;

	public static void SetDiscoverable(Activity activity, String unityObject, String unityMethod) {
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
			Intent discoverableIntent =	new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
			discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, 120);
			startActivityForResult(discoverableIntent, REQUEST_BLUETOOTH_DISCOVER);
		}
		else {
			Callback("true");
			CleanUp();
		}
	}

	@Override
	public void onActivityResult(int requestCode, int resultCode, Intent data) {
		if(requestCode == REQUEST_BLUETOOTH_DISCOVER) {
			if (resultCode != Activity.RESULT_CANCELED) {
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
