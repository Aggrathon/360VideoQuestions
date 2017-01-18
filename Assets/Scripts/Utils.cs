using UnityEngine;

public static class Utils {

	public struct HSVColorLerp
	{
		public float h, s, v, dH, dS, dV;

		public HSVColorLerp(Color a, Color b)
		{
			Color.RGBToHSV(a, out h, out s, out v);
			Color.RGBToHSV(b, out dH, out dS, out dV);
			dV = dV - v;
			dS = dS - s;
			dH = dH - h;
			if (dH < -0.5f) dH += 1f;
			if (dH > 0.5f) dH -= 1f;
		}

		public Color Lerp(float t)
		{
			return Color.HSVToRGB((h + t * dH + 1f) % 1f, s + t * dS, v + t * dV);
		}

		public float LerpScaling(float max)
		{
			return max * 0.5f + Mathf.Abs(max * dH);
		}
	}
}
