package aggrathon.vq360.javaplugin;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;
import java.util.Set;

public class BluetoothClient {

	public static final String TAG = "vq360_BLUETOOTH_CLIENT";

	BluetoothAdapter mBluetoothAdapter;
	BluetoothDevice mDevice;
	BluetoothSocket mSocket;
	InputStream mInStream;
	byte[] mBuffer;

	public BluetoothClient() {
		mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		mBuffer = new byte[1024];
	}

	public String GetDevices() {
		StringBuilder sb = new StringBuilder();
		for (BluetoothDevice d : mBluetoothAdapter.getBondedDevices()) {
			sb.append(d.getName().replace(',', '.'));
			sb.append(',');
		}
		if(sb.length() > 0) {
			return sb.substring(0, sb.length()-1);
		}
		else {
			return "";
		}
	}

	public void Connect(String device) {
		for (BluetoothDevice d : mBluetoothAdapter.getBondedDevices()) {
			if(d.getName().replace(',','.').equals(device)) {
				Connect(d);
				return;
			}
		}
	}

	public void Connect(BluetoothDevice device) {
		Close();
		try {
			mSocket = device.createRfcommSocketToServiceRecord(Bluetooth.UUID);
			try {
				mSocket.connect();
				try {
					mInStream = mSocket.getInputStream();
				} catch (IOException e) {
					Log.e(TAG, "Error occurred when creating input stream", e);
					Close();
				}
			} catch (IOException connectException) {
				Log.e(TAG, "Unable to connect", connectException);
				Close();
			}
		} catch (IOException e) {
			Log.e(TAG, "Socket's create() method failed", e);
		}
	}

	public String Listen() {
		if (mInStream != null) {
			int numBytes;

			try {
				numBytes = mInStream.read(mBuffer);
				return new String(mBuffer, 0, numBytes);
			} catch (IOException e) {
				Log.d(TAG, "Input stream was disconnected", e);
				return "";
			}
		}
		return "";
	}

	public void Close() {
		if (mSocket != null) {
			try {
				mSocket.close();
			} catch (IOException closeException) {
				Log.e(TAG, "Could not close the client socket", closeException);
			}
		}
		mInStream = null;
	}

}
