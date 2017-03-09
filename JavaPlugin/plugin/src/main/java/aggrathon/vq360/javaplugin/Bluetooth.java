package aggrathon.vq360.javaplugin;


import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

public class Bluetooth {

	public static final java.util.UUID UUID = java.util.UUID.fromString("9968f876-9d9a-48eb-8867-bbec1af63084");
	public static final String TAG = "vq360_BLUETOOTH_COM";


	BluetoothAdapter mBluetoothAdapter;
	private BluetoothServerSocket mServerSocket;
	private BluetoothSocket mSocket;
	private InputStream mInStream;
	private OutputStream mOutStream;
	private byte[] mBuffer;


	public Bluetooth() {
		mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
	}


	/**
	 * Wait for another device to connect
	 * This is a blocking call (should be called from a thread)
	 * @return
	 */
	public boolean ConnectServer() {
		Close();
		if(!mBluetoothAdapter.isEnabled())
			return false;
		try {
			mServerSocket = mBluetoothAdapter.listenUsingInsecureRfcommWithServiceRecord(TAG, UUID);
		} catch (IOException e) {
			Log.e(TAG, "Socket's listen() method failed", e);
		}
		if (mServerSocket == null)
			return false;
		try {
			if (SetSocket(mServerSocket.accept())) {
				try {
					mServerSocket.close();
				} catch (IOException e) {
					Log.e(TAG, "Could not close the server socket", e);
				}
				mServerSocket = null;
				return true;
			}
		} catch (IOException e) {
			Log.e(TAG, "Socket's accept() method failed", e);
		}
		Close();
		return false;
	}


	/**
	 * Get All Paired BluetoothSettings Devices
	 * @return csv of devices
	 */
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


	/**
	 * Connect to another device
	 * @param device
	 * @return was the connection successful
	 */
	public boolean ConnectClient(String device) {
		for (BluetoothDevice d : mBluetoothAdapter.getBondedDevices()) {
			if(d.getName().replace(',', '.').equals(device)) {
				return ConnectClient(d);
			}
		}
		return false;
	}


	/**
	 * Connect to another device
	 * @param device
	 * @return was the connection successful
	 */
	public boolean ConnectClient(BluetoothDevice device) {
		Close();
		if(!mBluetoothAdapter.isEnabled())
			return false;
		try {
			return SetSocket(device.createRfcommSocketToServiceRecord(UUID));
		} catch (IOException e) {
			Log.e(TAG, "Socket's create() method failed", e);
			return false;
		}
	}


	/**
	 * Sets the communication socket and opens io-streams
	 * @param socket
	 * @return operation successful
	 */
	private boolean SetSocket(BluetoothSocket socket) {
		if(socket == null)
			return false;
		mSocket = socket;
		try {
			mSocket.connect();
			try {
				mInStream = mSocket.getInputStream();
			} catch (IOException e) {
				Log.e(TAG, "Error occurred when creating input stream", e);
				Close();
				return false;
			}
			try {
				mOutStream = mSocket.getOutputStream();
			} catch (IOException e) {
				Log.e(TAG, "Could not get the output stream", e);
				Close();
				return false;
			}
		} catch (IOException connectException) {
			Log.e(TAG, "Unable to connect", connectException);
			Close();
			return false;
		}
		if(mBuffer == null) {
			mBuffer = new byte[1024];
		}
		return true;
	}


	/**
	 * Get Message from the connected device
	 * This is a blocking call (should be called from a thread)
	 * The devices should be connected before using this
	 * @return the message or empty on fail
	 */
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


	/**
	 * Send a message to the connected device
	 * The devices should be connected before using this
	 * @param data the message to send
	 */
	public void Send(String data) {
		if(mOutStream != null) {
			try {
				mOutStream.write(data.getBytes());
			} catch (IOException e) {
				Log.e(TAG, "Error occurred when sending data", e);
			}
		}
	}


	/**
	 * Close all connections between the devices
	 */
	public void Close() {
		if (mServerSocket != null) {
			try {
				mServerSocket.close();
				mServerSocket = null;
			} catch (IOException e) {
				Log.e(TAG, "Could not close the server socket", e);
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
		mInStream = null;
		mOutStream = null;
	}
}
