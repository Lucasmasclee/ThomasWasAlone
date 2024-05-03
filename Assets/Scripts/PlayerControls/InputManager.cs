using System;
using UnityEngine;

public class InputManager
{
    private PlayerActions PlayerActions;
    public event Action OnJump;
    public event Action OnBackwardCharacter;
    public event Action OnFowardCharacter;
    public event Action OnResetLevel;
    public event Action OnSplit;
    public event Action OnStick;
    public event Action OnUnstick;
    public Vector2 Movement => PlayerActions.PlayerControls.Movement.ReadValue<Vector2>();

    public InputManager()
    {
        PlayerActions = new PlayerActions();
        PlayerActions.PlayerControls.Enable();
        PlayerActions.PlayerControls.Jump.performed += (c) => OnJump?.Invoke();
        PlayerActions.PlayerControls.BackwardCharacter.performed +=  (c) => OnBackwardCharacter?.Invoke();
        PlayerActions.PlayerControls.FowardCharacter.performed +=  (c) => OnFowardCharacter?.Invoke();
        PlayerActions.PlayerControls.ResetLevel.performed +=  (c) => OnResetLevel?.Invoke();
        PlayerActions.PlayerControls.Split.performed +=  (c) => OnSplit?.Invoke();
        PlayerActions.PlayerControls.Stick.performed += (c) => OnStick?.Invoke();
    }
}
