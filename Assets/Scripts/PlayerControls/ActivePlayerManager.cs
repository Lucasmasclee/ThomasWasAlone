using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivePlayerManager : MonoBehaviour
{
    [SerializeField] private int SplitLimitBase;
    private List<PlayerController> playerControllers;
    private Vector2 inputMovement;
    private int controllerIndex;
    private CameraBehaviors cameraBehaviors;
    private SceneActions sceneActions;
    private PlayerController ActivePlayerController => playerControllers[controllerIndex];
    private int playersAtExitPortal;
    private bool frozenLevel;
    private Vector2 almostZero;
    private float shrinkSpeed;

    private void Awake()
    {
        Status.SetSplitLimitBase(SplitLimitBase);
        playersAtExitPortal = 0;
        controllerIndex = 0;
        shrinkSpeed = 0.5f;
        frozenLevel = false;
        playerControllers = GetComponentsInChildren<PlayerController>().ToList();
        foreach (PlayerController playerController in playerControllers)
        {
            playerController.SetStatus(0);
        }
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

    public void OnSplit(InputAction.CallbackContext value)
    {
        if (value.started && ActivePlayerController.CanSplit())
        {
            ActivePlayerController.Split();
            Vector2 newLocalScale = NewSplittedScale(ActivePlayerController.transform.localScale);
            ActivePlayerController.transform.DOScale(newLocalScale, shrinkSpeed);
            InstantiateNewChar(newLocalScale);
        }
    }

    private Vector2 NewSplittedScale(Vector2 startScale)
    {
        float newX = Mathf.Sqrt(startScale.x * startScale.x / 2);
        float newY = Mathf.Sqrt(startScale.y * startScale.y / 2);
        return new Vector2(newX, newY);
    }

    private void InstantiateNewChar(Vector2 newLocalScale)
    {
        PlayerController newChar = Instantiate(
                ActivePlayerController,
                ActivePlayerController.transform.position,
                Quaternion.identity,
                this.transform); // The new char will be a child of this object.
        newChar.SetStatus(ActivePlayerController.GetSplitFactor());
        playerControllers.Add(newChar);
        newChar.transform.DOScale(newLocalScale, shrinkSpeed);
    }

    private void CheckLevelComplete(int value)
    {
        playersAtExitPortal += value;

        if (playersAtExitPortal == playerControllers.Count && !frozenLevel)
        {
            frozenLevel = true;
            foreach (PlayerController playerController in playerControllers)
            {
                playerController.transform.DOScale(almostZero, 1f);
            }
            Invoke(nameof(CompleteLevel), 2);
        }
    }

    private void CompleteLevel()
    {
        sceneActions.LoadNextScene();
    }

    private void IncrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex + 1) % playerControllers.Count;
        SelectNewChar(ActivePlayerController);
    }

    private void DecrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex - 1 + playerControllers.Count) % playerControllers.Count;
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

    public void MergeObjects(Vector3 pos1, Vector3 pos2, float scale)
    {
        ActivePlayerController.Merge();
        Vector3 spawnPos = (pos1 + pos2) / 2f; // Spawn pos will be in the middle of the two objects
        PlayerController newObj = Instantiate(ActivePlayerController, spawnPos, Quaternion.identity); // Instantiate the object
        newObj.transform.localScale = new Vector3(scale, scale, 0f);
    }
}
