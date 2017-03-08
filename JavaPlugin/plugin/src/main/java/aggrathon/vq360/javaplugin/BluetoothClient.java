package aggrathon.vq360.javaplugin;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;

public class BluetoothClient {

	public static final String TAG = "vq360_BLUETOOTH_CLIENT";

	BluetoothDevice mDevice;
	BluetoothSocket mSocket;
	InputStream mInStream;
	byte[] mBuffer;

	public BluetoothClient(BluetoothDevice device) {
		mDevice = device;
		mBuffer = new byte[1024];
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
