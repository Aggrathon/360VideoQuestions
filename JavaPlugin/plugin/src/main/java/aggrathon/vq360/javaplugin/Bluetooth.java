package aggrathon.vq360.javaplugin;


import android.app.Activity;
import android.bluetooth.BluetoothAdapter;
import android.content.Intent;

public class Bluetooth extends UnityFragment {

	public static final java.util.UUID UUID = java.util.UUID.fromString("9968f876-9d9a-48eb-8867-bbec1af63084");
	private static final int REQUEST_BLUETOOTH_ON = 226;
	private static final int REQUEST_BLUETOOTH_DISCOVER = 4355;

	public static void SetEnabled(Activity activity, String unityObject, String unityMethod) {
		Bluetooth bt = new Bluetooth();
		bt.setDiscoverable = false;
		CallFragment(bt, activity, unityObject, unityMethod);
	}

	public static void SetDiscoverable(Activity activity, String unityObject, String unityMethod) {
		Bluetooth bt = new Bluetooth();
		bt.setDiscoverable = true;
		CallFragment(bt, activity, unityObject, unityMethod);
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
