using System;
using UnityEngine;

public class MemoryGameClickHandler : MonoBehaviour
{
	public event Action OnClicked = () => {};

	[SerializeField]
	private Camera TargetCamera;

	[SerializeField]
	private LayerMask ClickableLayers = ~0;

	private void Update()
	{
		if (Input.touchCount <= 0)
		{
            HandleMouseInput();
			return;
        }

		HandleTouchInput();

    }

    private void HandleTouchInput()
    {
		Touch touch = Input.GetTouch(0);
		if (touch.phase == TouchPhase.Began)
		{
			TryHandlePointer(touch.position);
		}
	}

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            TryHandlePointer(mousePosition);
        }
    }

    public void Enable()
    {
        this.enabled = true;
    }

    public void Disable()
    {
        this.enabled = false;
    }

	private void TryHandlePointer(Vector2 screenPosition)
	{
		Ray ray = TargetCamera.ScreenPointToRay(screenPosition);

		if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ClickableLayers))
		{
			var collider = hit.collider;

			var model = collider.gameObject.GetComponent<MemoryGameBlockModel>();
			model.HandleClicked();
		}
	}
}
