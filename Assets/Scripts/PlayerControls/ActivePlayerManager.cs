using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ActivePlayerManager : MonoBehaviour
{
    [SerializeField] private int SplitLimitBase;
    private List<PlayerController> playerControllers;
    private int controllerIndex;
    private CameraBehaviors cameraBehaviors;
    private SceneActions sceneActions;
    private PlayerController ActivePlayerController;
    private float shrinkSpeed;
    private Vector2 inputMovement => GameManager.Instance.InputManager.Movement;

    private void Awake()
    {
        Status.SetSplitLimitBase(SplitLimitBase);
        controllerIndex = 0;
        shrinkSpeed = 0.5f;
        playerControllers = GetComponentsInChildren<PlayerController>().ToList();
        foreach (PlayerController playerController in playerControllers)
        {
            playerController.SetStatus(0);
        }
        cameraBehaviors = GetComponentInChildren<CameraBehaviors>();
        sceneActions = new SceneActions();
    }

    private void Start()
    {
        GameManager.Instance.InputManager.OnFowardCharacter += OnFowardCharacter;
        GameManager.Instance.InputManager.OnBackwardCharacter += OnBackwardCharacter;
        GameManager.Instance.InputManager.OnResetLevel += OnResetLevel;
        GameManager.Instance.InputManager.OnSplit += OnSplit;
        GameManager.Instance.InputManager.OnJump += OnJump;
        GameManager.Instance.InputManager.OnStick += OnStick;
        GameManager.Instance.InputManager.OnSelfkill += OnSelfkill;
        ActivePlayerController = playerControllers.First();
        SelectNewChar();
    }

    private void OnDestroy()
    {
        GameManager.Instance.InputManager.OnFowardCharacter -= OnFowardCharacter;
        GameManager.Instance.InputManager.OnBackwardCharacter -= OnBackwardCharacter;
        GameManager.Instance.InputManager.OnResetLevel -= OnResetLevel;
        GameManager.Instance.InputManager.OnSplit -= OnSplit;
        GameManager.Instance.InputManager.OnJump -= OnJump;
        GameManager.Instance.InputManager.OnStick -= OnStick;
        GameManager.Instance.InputManager.OnSelfkill += OnSelfkill;
    }

    private void FixedUpdate()
    {
        Stack<PlayerController> playerControllersToDie = new Stack<PlayerController>();

        foreach (PlayerController playerController in playerControllers)
        {
            if (!playerController.IsDead) continue;

            playerControllersToDie.Push(playerController);
            playerController.transform.DOKill();

            if (playerController.GetInstanceID() == ActivePlayerController.GetInstanceID())
            {
                IncrementIndex();
            }
        }

        if (playerControllers.Count - playerControllersToDie.Count <= 0)
        {
            sceneActions.ResetLevel();
            return;
        }

        foreach (PlayerController playerControllerToDie in  playerControllersToDie)
        {
            playerControllers.Remove(playerControllerToDie);
            Object.Destroy(playerControllerToDie.gameObject);
        }

        ActivePlayerController.Move();
        ActivePlayerController.OnMovement(inputMovement);
    }

    private void OnFowardCharacter()
    {
        if (playerControllers.Count != 1)
        {
            IncrementIndex();
        }
    }

    private void OnBackwardCharacter()
    {
        if (playerControllers.Count != 1)
        {
            DecrementIndex();
        }
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

    private void OnSelfkill()
    {
        if(playerControllers.Count > 1)
        {
            ActivePlayerController.TakeDamage(30);
        }
    }

    private void OnStick()
    {
        ActivePlayerController.OnStick();
    }

    private void InstantiateNewChar(Vector2 newLocalScale)
    {
        PlayerController newChar = Instantiate(
                ActivePlayerController,
                ActivePlayerController.transform.position,
                Quaternion.identity,
                this.transform);
        newChar.SetStatus(ActivePlayerController.GetSplitFactor());
        playerControllers.Add(newChar);
        newChar.transform.DOScale(newLocalScale, shrinkSpeed);
    }

    private void IncrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex + 1) % playerControllers.Count;
        ActivePlayerController = playerControllers[controllerIndex];
        SelectNewChar();
    }

    private void DecrementIndex()
    {
        ActivePlayerController.StopMovement();
        controllerIndex = (controllerIndex - 1 + playerControllers.Count) % playerControllers.Count;
        ActivePlayerController = playerControllers[controllerIndex];
        SelectNewChar();
    }

    public void OnJump()
    {
        ActivePlayerController.OnJump();
    }

    private void SelectNewChar()
    {
        ActivePlayerController = playerControllers[controllerIndex];
        ActivePlayerController.OnMovement(inputMovement);
        cameraBehaviors.FollowChar(ActivePlayerController.transform);
    }

    public void MergeObjects(Vector3 pos1, Vector3 pos2, float scale)
    {
        //ActivePlayerController.Merge();
        Vector3 spawnPos = (pos1 + pos2) / 2f;
        PlayerController newObj = Instantiate(ActivePlayerController, spawnPos, Quaternion.identity);
        newObj.transform.localScale = new Vector3(scale, scale, 0f);
    }
}
