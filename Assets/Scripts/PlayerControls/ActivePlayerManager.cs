using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActivePlayerManager : MonoBehaviour
{
    [SerializeField] private int SplitLimitBase;
    private List<PlayerController> playerControllers;
    private int controllerIndex;
    private CameraBehaviors cameraBehaviors;
    private SceneActions sceneActions;
    private PlayerController ActivePlayerController => playerControllers[controllerIndex];
    private int playersAtExitPortal;
    private bool frozenLevel;
    private Vector2 almostZero;
    private float shrinkSpeed;
    private InputManager inputManager;
    private Vector2 inputMovement => inputManager.Movement;

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
        almostZero = new Vector2(0.1f, 0.1f);
        inputManager = new InputManager();
    }

    private void Start()
    {
        inputManager.OnFowardCharacter += OnFowardCharacter;
        inputManager.OnBackwardCharacter += OnBackwardCharacter;
        inputManager.OnResetLevel += OnResetLevel;
        inputManager.OnSplit += OnSplit;
        inputManager.OnJump += OnJump;
        SelectNewChar(ActivePlayerController);
    }

    private void OnEnable()
    {
        ExitPortal.OnStepOnExitPortal += CheckLevelComplete;
    }

    private void OnDisable()
    {
        ExitPortal.OnStepOnExitPortal -= CheckLevelComplete;
        inputManager.OnFowardCharacter -= OnFowardCharacter;
        inputManager.OnBackwardCharacter -= OnBackwardCharacter;
        inputManager.OnResetLevel -= OnResetLevel;
        inputManager.OnSplit -= OnSplit;
        inputManager.OnJump -= OnJump;
    }

    private void FixedUpdate()
    {
        if (!ActivePlayerController.Move())
        {
            playerControllers.Remove(ActivePlayerController);
            if (playerControllers.Count <= 0)
            {
                sceneActions.ResetLevel();
                return;
            }
            IncrementIndex();
        }
        ActivePlayerController.OnMovement(inputMovement);
    }

    private void OnFowardCharacter()
    {
        IncrementIndex();
    }

    private void OnBackwardCharacter()
    {
        DecrementIndex();
    }

    private void OnResetLevel()
    {
        sceneActions.ResetLevel();
    }

    private void OnSplit()
    {
        if (ActivePlayerController.CanSplit())
        {
            ActivePlayerController.Split();
            Vector2 newLocalScale = ActivePlayerController.ShrinkToNewScale(shrinkSpeed);
            InstantiateNewChar(newLocalScale);
        }
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

    public void OnJump()
    {
        ActivePlayerController.OnJump();
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
