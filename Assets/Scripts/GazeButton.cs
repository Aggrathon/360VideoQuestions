using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

public class GazeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IPointerClickHandler, IDeselectHandler, ISubmitHandler {

	public float activationTime = 3f;
	public Image progressbar;

	public System.Action onActivation;

	private float progress = 0f;
	private bool progressing = false;

	void OnEnable()
	{
		progress = 0f;
		progressing = false;
	}

	public void OnDeselect(BaseEventData eventData)
	{
		progressing = false;
	}

	public void OnPointerClick(PointerEventData eventData)
	{
		if (EventSystem.current.currentSelectedGameObject == gameObject)
		{
			Activate();
		}
	}

	public void OnPointerEnter(PointerEventData eventData)
	{
		EventSystem.current.SetSelectedGameObject(gameObject);
		progressing = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		progressing = false;
	}

	public void OnSelect(BaseEventData eventData)
	{
		progressing = true;
	}

	public void OnSubmit(BaseEventData eventData)
	{
		if (EventSystem.current.currentSelectedGameObject == gameObject)
		{
			Activate();
		}
	}

	void Update ()
	{
		if (progressing)
		{
			progress += Time.deltaTime / activationTime;
			if(progress > 1f)
			{
				Activate();
			}
			else
			{
				progressbar.fillAmount = progress;
			}
		}
		else if(progress != 0f)
		{
			progress -= Time.deltaTime / activationTime * 2f;
			if (progress < 0f)
			{
				progress = 0f;
			}
			progressbar.fillAmount = progress;
		}
	}

	void Activate()
	{
		if(onActivation != null)
		{
			onActivation();
		}
		else
		{
			Debug.Log("Empty Activation");
		}
		progress = 1f;
		progressbar.fillAmount = progress;
		progressing = false;
	}
}
