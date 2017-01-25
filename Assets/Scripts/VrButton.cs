using System;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class VrButton : MonoBehaviour {

	public Text text;
	public Image image;

	Action onClick;

	public void Setup(string text, string image, Action onClick)
	{
		if(text != "")
		{
			this.text.text = text;
			this.text.gameObject.SetActive(true);
		}
		else
		{
			this.text.gameObject.SetActive(false);
		}
		if (image != "")
		{
			try
			{
				byte[] data = File.ReadAllBytes(image);
				Texture2D tex = new Texture2D(0, 0);
				tex.LoadImage(data, true);
				this.image.preserveAspect = true;
				this.image.type = Image.Type.Simple;
				this.image.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
				this.image.gameObject.SetActive(true);
			}
			catch (Exception e)
			{
				DebugText.LogError("Could not load image '" + image + "' (" + e.GetType().ToString() + ")");
				this.image.gameObject.SetActive(false);
			}
		}
		else
		{
			this.image.gameObject.SetActive(false);
		}
		this.onClick = onClick;
	}

	public void Click()
	{
		if (onClick != null)
			onClick();
	}
}
