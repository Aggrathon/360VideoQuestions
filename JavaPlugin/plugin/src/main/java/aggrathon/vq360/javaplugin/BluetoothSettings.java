package aggrathon.vq360.javaplugin;


import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;


public class BluetoothSettings extends UnityFragment {

	private static final int REQUEST_BLUETOOTH_ON = 226;
	private static final int REQUEST_BLUETOOTH_DISCOVER = 4355;

	public static void SetEnabled(Activity activity, String unityObject, String unityMethod) {
		BluetoothSettings bt = new BluetoothSettings();
		bt.setDiscoverable = false;
		CallFragment(bt, activity, unityObject, unityMethod);
	}

	public static void SetDiscoverable(Activity activity, String unityObject, String unityMethod) {
		BluetoothSettings bt = new BluetoothSettings();
		bt.setDiscoverable = true;
		CallFragment(bt, activity, unityObject, unityMethod);
	}

	public static void OpenSettings(Activity activity) {
		Intent intentOpenBluetoothSettings = new Intent();
		intentOpenBluetoothSettings.setAction(android.provider.Settings.ACTION_BLUETOOTH_SETTINGS);
		activity.startActivity(intentOpenBluetoothSettings);
	}


	private boolean setDiscoverable;

	@Override
	public void onStart() {
		super.onStart();
		BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		if (mBluetoothAdapter == null) {
			Callback("Device does not support Bluetooth");
			CleanUp();
		}
		else {
			if (setDiscoverable) {
				Intent discoverableIntent =	new Intent(BluetoothAdapter.ACTION_REQUEST_DISCOVERABLE);
				discoverableIntent.putExtra(BluetoothAdapter.EXTRA_DISCOVERABLE_DURATION, 120);
				startActivityForResult(discoverableIntent, REQUEST_BLUETOOTH_DISCOVER);
			}
			else {
				if (!mBluetoothAdapter.isEnabled()) {
					Intent enableBtIntent = new Intent(BluetoothAdapter.ACTION_REQUEST_ENABLE);
					startActivityForResult(enableBtIntent, REQUEST_BLUETOOTH_ON);
				}
				else {
					Callback("true");
					CleanUp();
				}
			}

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
		else if(requestCode == REQUEST_BLUETOOTH_ON) {
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
