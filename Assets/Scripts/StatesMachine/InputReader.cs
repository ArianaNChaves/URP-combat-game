using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputReader : MonoBehaviour, Controls.IPlayerActions
{
    public event Action RolEvent;
    public event Action<bool> RunEvent;
        
    public Vector2 MovementValue {get; private set;}    
        
    private Controls _controls;
    private void Start()
    {
        _controls = new Controls();
        _controls.Player.SetCallbacks(this);
        
        _controls.Player.Enable();
    }

    private void OnDestroy()
    {
        _controls.Player.Disable();
    }


    public void OnRol(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RolEvent?.Invoke(); 
        }
        
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        MovementValue = context.ReadValue<Vector2>();
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            RunEvent?.Invoke(true); 
        }
        else if (context.canceled)
        {
            RunEvent?.Invoke(false);
        }
    }
}
