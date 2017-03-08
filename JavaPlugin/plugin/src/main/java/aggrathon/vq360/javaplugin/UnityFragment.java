package aggrathon.vq360.javaplugin;

import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;

import com.unity3d.player.UnityPlayer;

public abstract class UnityFragment extends Fragment {
	private String mUnityCallbackName;
	private String mUnityCallbackMethod;
	private FragmentManager mFragmentManager;

	public static void CallFragment(UnityFragment frag, Activity activity, String unityObjectName, String unityMethodName) {
		try {
			frag.mFragmentManager = activity.getFragmentManager();
			frag.mUnityCallbackName = unityObjectName;
			frag.mUnityCallbackMethod = unityMethodName;

			FragmentTransaction ft = frag.mFragmentManager.beginTransaction();
			ft.add(0, frag);
			ft.commit();
		}
		catch (Exception e) {
			frag.Callback(e.toString());
		}
	}

	public void Callback(String data) {
		UnityPlayer.UnitySendMessage(mUnityCallbackName, mUnityCallbackMethod, data);
	}

	public void CleanUp() {
		FragmentTransaction ft = mFragmentManager.beginTransaction();
		ft.remove(this);
		ft.commit();
	}

}
