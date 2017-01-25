using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class Utils {

	public struct HSVColorLerp
	{
		public float h, s, v, a, dH, dS, dV, dA;

		public HSVColorLerp(Color c1, Color c2)
		{
			Color.RGBToHSV(c1, out h, out s, out v);
			Color.RGBToHSV(c2, out dH, out dS, out dV);
			dV = dV - v;
			dS = dS - s;
			dH = dH - h;
			if (dH < -0.5f) dH += 1f;
			if (dH > 0.5f) dH -= 1f;
			a = c1.a;
			dA = c2.a - c1.a;
		}

		public Color Lerp(float t)
		{
			Color c = Color.HSVToRGB((h + t * dH + 1f) % 1f, s + t * dS, v + t * dV);
			c.a = a + t * dA;
			return c;
		}

		public float LerpScaling(float max)
		{
			return max * 0.5f + Mathf.Abs(max * dH);
		}
	}

	public static Texture2D LoadImage(string path)
	{
		Texture2D tex = new Texture2D(0,0);
		try
		{
			byte[] data = File.ReadAllBytes(path);
			tex.LoadImage(data, true);
		}
		catch (Exception e)
		{
			DebugText.LogError("Could not load image '" + path + "' (" + e.GetType().ToString() + ")");
		}
		return tex;
	}

	public static IEnumerator RunLater(Action action, IEnumerator delay)
	{
		yield return delay;
		action();
	}

	public static IEnumerator RunLater(Action action, YieldInstruction delay)
	{
		yield return delay;
		action();
	}
}
