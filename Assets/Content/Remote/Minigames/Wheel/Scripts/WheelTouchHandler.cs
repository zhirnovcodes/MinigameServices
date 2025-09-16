using System;
using UnityEngine;

public class WheelTouchHandler : MonoBehaviour
{
    [SerializeField] private float MaxSwipeSpeed = 10f;
    [SerializeField] private LayerMask WheelLayerMask = -1;
    
    public event Action WheelTouched = () => { };
    public event Action WheelReleased = () => { };
    
    private Camera MainCamera;
    private bool IsTouchingWheel = false;
    private Vector2 LastTouchPosition;
    private Vector2 CurrentTouchPosition;
    private Vector2 TouchDirection;

    private void Awake()
    {
        MainCamera = Camera.main;
        if (MainCamera == null)
            MainCamera = FindObjectOfType<Camera>();
    }
    
    private void Update()
    {
        HandleTouchInput();
    }
    
    private void HandleTouchInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            
            switch (touch.phase)
            {
                case TouchPhase.Began:
                    HandleTouchBegan(touch);
                    break;
                    
                case TouchPhase.Moved:
                case TouchPhase.Stationary:
                    HandleTouchMoved(touch);
                    break;
                    
                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    HandleTouchEnded();
                    break;
            }
        }
        else
        {
            // Handle mouse input for testing in editor
            HandleMouseInput();
        }
    }
    
    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Input.mousePosition;
            if (IsRaycastHitWheel(mousePosition))
            {
                IsTouchingWheel = true;
                LastTouchPosition = mousePosition;
                CurrentTouchPosition = mousePosition;
                WheelTouched();
            }
        }
        else if (Input.GetMouseButton(0) && IsTouchingWheel)
        {
            CurrentTouchPosition = Input.mousePosition;
        }
        else if (Input.GetMouseButtonUp(0) && IsTouchingWheel)
        {
            HandleTouchEnded();
        }
    }
    
    private void HandleTouchBegan(Touch touch)
    {
        if (IsRaycastHitWheel(touch.position))
        {
            IsTouchingWheel = true;
            LastTouchPosition = touch.position;
            CurrentTouchPosition = touch.position;
            WheelTouched();
        }
    }
    
    private void HandleTouchMoved(Touch touch)
    {
        if (IsTouchingWheel)
        {
            CurrentTouchPosition = touch.position;
        }
    }
    
    private void HandleTouchEnded()
    {
        if (IsTouchingWheel)
        {
            IsTouchingWheel = false;
            WheelReleased();
        }
    }
    
    private bool IsRaycastHitWheel(Vector2 screenPosition)
    {   
        Ray ray = MainCamera.ScreenPointToRay(screenPosition);
        
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, WheelLayerMask))
        {
            return true;
        }
        
        return false;
    }
    
    public Vector2 GetTouchDirection()
    {
        if (!IsTouchingWheel)
            return Vector2.zero;
            
        // Calculate direction vector
        TouchDirection = CurrentTouchPosition - LastTouchPosition;
        
        // Calculate speed and normalize to 0-1 range
        float speed = TouchDirection.magnitude;
        float normalizedSpeed = Mathf.Clamp01(speed / MaxSwipeSpeed);
        
        // Return normalized direction with speed as magnitude
        Vector2 normalizedDirection = TouchDirection.normalized;
        return normalizedDirection * normalizedSpeed;
    }
    
    public bool GetIsTouchingWheel()
    {
        return IsTouchingWheel;
    }
}
