package aggrathon.vq360.javaplugin;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

import java.io.IOException;
import java.io.OutputStream;

public class BluetoothServer {

	public static final String TAG = "vq360_BLUETOOTH_SERVER";

	BluetoothAdapter mBluetoothAdapter;
	BluetoothServerSocket mServerSocket;
	BluetoothSocket mSocket;
	OutputStream mOutStream;

	public BluetoothServer() {
		mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		if (mBluetoothAdapter != null) {
			try {
				mServerSocket = mBluetoothAdapter.listenUsingInsecureRfcommWithServiceRecord(TAG, Bluetooth.UUID);
			}
			catch (IOException e) {
				Log.e(TAG, "Socket's listen() method failed", e);
			}
		}
	}

	public boolean Connect() {
		if(mServerSocket == null) {
			return false;
		}
		try {
			mSocket = mServerSocket.accept();
		} catch (IOException e) {
			Log.e(TAG, "Socket's accept() method failed", e);
		}

		if (mSocket != null) {
			try {
				mOutStream = mSocket.getOutputStream();
			} catch (IOException e) {
				Log.e(TAG, "Could not get the output stream", e);
			}
			try {
				mServerSocket.close();
			} catch (IOException e) {
				Log.e(TAG, "Could not close the connect socket", e);
			}
			mServerSocket = null;
			return true;
		}

		Close();
		return false;
	}

	public void Close() {
		if (mServerSocket != null) {
			try {
				mServerSocket.close();
				mServerSocket = null;
			} catch (IOException e) {
				Log.e(TAG, "Could not close the connect socket", e);
			}
		}
		if (mSocket != null) {
			try {
				mSocket.close();
				mSocket = null;
			} catch (IOException e) {
				Log.e(TAG, "Could not close the communication socket", e);
			}
		}
		mOutStream = null;
	}

	public void Send(String data) {
		if(mOutStream != null) {
			try {
				mOutStream.write(data.getBytes());
			} catch (IOException e) {
				Log.e(TAG, "Error occurred when sending data", e);
			}

		}
	}

}
