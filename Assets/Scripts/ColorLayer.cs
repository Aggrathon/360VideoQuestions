using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ColorLayer : MonoBehaviour {

	public bool cycleColors;

	new MeshRenderer renderer;

	void Awake () {
		renderer = GetComponent<MeshRenderer>();
		if(cycleColors)
		{
			StartCoroutine(ColorCycle());
		}
	}

	IEnumerator FadeOut(float speed)
	{
		Color c = renderer.material.color;
		c.a = 0;
		yield return ChangeColor(c, speed, false);
		gameObject.SetActive(false);
	}

	IEnumerator FadeInOut(Color color, float speed)
	{
		yield return ChangeColor(color, speed * 0.5f, false);
		yield return FadeOut(speed*0.5f);
	}

	IEnumerator ChangeColor(Color newColor, float speed, bool speedScaling = false)
	{
		Utils.HSVColorLerp lerp = new Utils.HSVColorLerp(renderer.material.color, newColor);
		float time = 0;
		if (speedScaling) speed = lerp.LerpScaling(speed);
		while (time < speed)
		{
			yield return null;
			time += Time.deltaTime;
			renderer.material.color = lerp.Lerp(time / speed);
		}
		renderer.material.color = newColor;
	}

	IEnumerator ColorCycle()
	{
		while(true)
		{
			Color c = Random.ColorHSV(0, 1, 0.2f, 1, 0.3f, 1, 1, 1);
			yield return ChangeColor(c, 2f, true);
		}
	}


	public void TurnOn(float speed)
	{
		TurnOn(new Color(0, 0, 0, 1), speed);
	}

	public void TurnOn(Color color, float speed)
	{
		if(!gameObject.activeSelf)
		{
			renderer.material.color = new Color(0, 0, 0, 0);
			gameObject.SetActive(true);
		}
		else
		{
			StopAllCoroutines();
		}
		StartCoroutine(ChangeColor(color, speed, false));
	}

	public void TurnOff(float speed)
	{
		if(gameObject.activeSelf)
		{
			StopAllCoroutines();
			StartCoroutine(FadeOut(speed));
		}
	}

	public void Flash(float speed)
	{
		Flash(new Color(0, 0, 0, 1), speed);
	}

	public void Flash(Color color, float speed)
	{
		if(gameObject.activeSelf)
		{
			StopAllCoroutines();
		}
		else
		{
			renderer.material.color = new Color(0, 0, 0, 0);
			gameObject.SetActive(true);
		}
		StartCoroutine(FadeInOut(color, speed));
	}

	public void SetColor(Color color, float speed)
	{
		if (!gameObject.activeSelf)
			TurnOn(color, speed);
		else
		{
			StopAllCoroutines();
			StartCoroutine(ChangeColor(color, speed, true));
		}
	}
}
