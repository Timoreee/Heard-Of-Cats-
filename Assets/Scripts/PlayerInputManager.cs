using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    //Properties
    public Vector2 LookInput {get; private set; }
    public bool PlayerIsSprinting {get; private set; }
    public Vector2 MovementInput {get; private set; }

    //jump and dash actions
    public event Action OnJumpButtonPressed;
    public event Action OnJumpButtonReleased;
    public event Action OnDashButtonPressed;

    // Interactable actions
    public event Action OnInteractButtonPressed;
    public event Action OnAttackButtonPressed;
    public event Action OnMeowButtonPressed;


    //grabs the vector 2 when the player looks with the mouse
    private void OnLook(InputValue inputValue)
    {
        LookInput = inputValue.Get<Vector2>();
    }
    //grabs the vector 2 when the player moves
    private void OnMove(InputValue inputValue)
    {
        MovementInput = inputValue.Get<Vector2>();
    }
    //tells when the jump button is pressed
    private void OnJump(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            OnJumpButtonPressed?.Invoke();
        }
        else
        {
            OnJumpButtonReleased?.Invoke();
        }
    }
    //tells when the dash button is presed
    private void OnDash(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            OnDashButtonPressed?.Invoke();
        }
    }
    //tells when the sprint button is pressed
    private void OnSprint(InputValue inputValue)
    {
        PlayerIsSprinting = inputValue.isPressed;
    }
    private void OnInteract(InputValue inputValue)
    {
        if (inputValue.isPressed)
        {
            OnInteractButtonPressed?.Invoke();
        }
    }
    private void OnAttack(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
            OnAttackButtonPressed?.Invoke();
        }
    }
    private void OnMeow(InputValue inputValue)
    {
        if(inputValue.isPressed)
        {
            OnMeowButtonPressed?.Invoke();
        }
    }
}
