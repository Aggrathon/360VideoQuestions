package aggrathon.vq360.javaplugin;

import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothServerSocket;
import android.bluetooth.BluetoothSocket;
import android.util.Log;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.concurrent.ConcurrentLinkedQueue;

public class Bluetooth {

	public static final java.util.UUID UUID = java.util.UUID.fromString("9968f876-9d9a-48eb-8867-bbec1af63084");
	public static final String TAG = "vq360_BLUETOOTH_COM";


	BluetoothAdapter mBluetoothAdapter;
	private BluetoothServerSocket mServerSocket;

	private BluetoothSocket mSocket;
	private InputStream mInStream;
	private OutputStream mOutStream;
	private byte[] mBuffer;

	private Thread mThread;
	private ConcurrentLinkedQueue<String> mQueue;


	public Bluetooth() {
		mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
		mQueue = new ConcurrentLinkedQueue<String>();
	}


	/**
	 * Wait for another device to connect (async)
	 * Call GetMessage() to know if a connection has been established
	 * @return
	 */
	public void ConnectServer() {
		Close();
		if(!mBluetoothAdapter.isEnabled()) {
			mQueue.add("Bluetooth needed");
			return;
		}
		try {
			mServerSocket = mBluetoothAdapter.listenUsingRfcommWithServiceRecord(TAG, UUID);
		} catch (IOException e) {
			Log.e(TAG, "Socket's listen() method failed", e);
		}
		if (mServerSocket == null) {
			mQueue.add("Bluetooth socket failed");
			return;
		}
		mThread = new Thread() {
			@Override
			public void run() {
				try {
					if (mServerSocket != null && SetSocket(mServerSocket.accept())) {
						try {
							mServerSocket.close();
						} catch (IOException e) {
							Log.e(TAG, "Could not close the server socket", e);
						}
						mServerSocket = null;
						mQueue.add("connected");
					}
					else {
						mQueue.add("Could not connect");
					}
				} catch (IOException e) {
					Log.e(TAG, "Socket's accept() method failed", e);
					mQueue.add("Bluetooth connection failed");
				}
			}
		};
		mThread.start();
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
	 * Try get a received message
	 * If waiting for a connection then "connected" or an error
	 *  will be the first message after a connection attempt
	 * @return message or null if none
	 */
	public String GetMessage() {
		return mQueue.poll();
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
		StartListenerThread();
		return true;
	}


	/**
	 * Starts a thread listening for incoming messages
	 * The devices must be connected first
	 */
	private void StartListenerThread() {
		mThread = new Thread() {
			@Override
			public void run() {
				while (true) {
					if (mInStream != null) {
						int numBytes;
						try {
							numBytes = mInStream.read(mBuffer);
							mQueue.add(new String(mBuffer, 0, numBytes));
						} catch (IOException e) {
							Log.d(TAG, "Input stream was disconnected", e);
							return;
						}
					}
				}
			}
		};
		mThread.start();
	}


	/**
	 * Close all connections between the devices
	 */
	public void Close() {
		try {
			if (mServerSocket != null) {
				mServerSocket.close();
				mServerSocket = null;
			}
		} catch(IOException e){
			Log.e(TAG, "Could not close the server socket", e);
		}
		try {
			if (mSocket != null) {
				mSocket.close();
				mSocket = null;
			}
		} catch (IOException e) {
			Log.e(TAG, "Could not close the communication socket", e);
		}
		if(mThread != null && mThread.isAlive()) {
			mThread.interrupt();
		}
		mQueue.clear();
		mInStream = null;
		mOutStream = null;
	}
}
