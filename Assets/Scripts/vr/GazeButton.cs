using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class GazeButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IPointerClickHandler, IDeselectHandler, ISubmitHandler {

	public float activationTime = 3f;
	public Image progressbar;

	public UnityEvent onActivate;

	private float progress = -1f;
	private bool progressing = false;

	void OnEnable()
	{
		progress = -1f;
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
		DebugText.LogError("enter_"+eventData.currentInputModule.GetType().ToString());
		EventSystem.current.SetSelectedGameObject(gameObject);
		progressing = true;
	}

	public void OnPointerExit(PointerEventData eventData)
	{
		DebugText.LogError("exit_" + eventData.currentInputModule.GetType().ToString());
		progressing = false;
	}

	public void OnSelect(BaseEventData eventData)
	{
		DebugText.LogError("select_" + eventData.currentInputModule.GetType().ToString());
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
		onActivate.Invoke();
		progress = 1f;
		progressbar.fillAmount = progress;
		progressing = false;
	}
}
