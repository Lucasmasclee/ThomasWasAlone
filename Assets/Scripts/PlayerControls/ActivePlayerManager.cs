using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivePlayerManager : MonoBehaviour
{
    private PlayerController[] playerControllers;
    private Vector2 inputMovement;
    private int controllerIndex;
    private CameraBehaviors cameraBehaviors;
    private SceneActions sceneActions;
    private PlayerController ActivePlayerController => playerControllers[controllerIndex];
    private int playersAtExitPortal;
    private bool frozenLevel;
    private Vector2 almostZero;

    private void Awake()
    {
        playersAtExitPortal = 0;
        controllerIndex = 0;
        frozenLevel = false;
        playerControllers = GetComponentsInChildren<PlayerController>();
        cameraBehaviors = GetComponentInChildren<CameraBehaviors>();
        sceneActions = new SceneActions();
        inputMovement = Vector2.zero;
        almostZero = new Vector2(0.1f, 0.1f);
    }

    private void Start()
    {
        SelectNewChar(ActivePlayerController);
    }

    private void OnEnable()
    {
        ExitPortal.OnStepOnExitPortal += CheckLevelComplete;
    }

    private void OnDisable()
    {
        ExitPortal.OnStepOnExitPortal -= CheckLevelComplete;
    }

    private void FixedUpdate()
    {
        ActivePlayerController.Move();
    }

    public void OnFowardCharacter(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            IncrementIndex();
        }
    }

    public void OnBackwardCharacter(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            DecrementIndex();
        }
    }

    public void OnResetLevel(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            sceneActions.ResetLevel();
        }
    }

    private void CheckLevelComplete(int value)
    {
        playersAtExitPortal += value;

        if (playersAtExitPortal == playerControllers.Length && !frozenLevel)
        {
            frozenLevel = true;
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.transform.DOScale(almostZero, 1f);
            }
            Invoke("CompleteLevel", 2);
        }
    }

    private void CompleteLevel()
    {
        sceneActions.LoadNextScene();
    }

    private void IncrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex + 1) % playerControllers.Length;
        SelectNewChar(ActivePlayerController);
    }

    private void DecrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex - 1 + playerControllers.Length) % playerControllers.Length;
        SelectNewChar(ActivePlayerController);
    }

    public void OnMovement(InputAction.CallbackContext value)
    {
        inputMovement = value.ReadValue<Vector2>();
        ActivePlayerController.OnMovement(inputMovement);
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            ActivePlayerController.OnJump();
        }
    }

    private void SelectNewChar(PlayerController playerController)
    {
        playerController.OnMovement(inputMovement);
        cameraBehaviors.FollowChar(playerController.transform);
    }
}
