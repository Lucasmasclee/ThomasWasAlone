using DG.Tweening;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private Transform spriteTransform;
    private SceneActions sceneActions;
    private Vector2 minPlayerScale;

    private void Awake()
    {
        spriteTransform = GetComponent<SpriteRenderer>().transform;
        sceneActions = new SceneActions();
        minPlayerScale = new Vector2(0.1f, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController playerController = collision.GetComponent<PlayerController>();

        if (playerController != null)
        {
            playerController.transform.DOScale(minPlayerScale, 0.5f);
            spriteTransform.DOScale(Vector2.zero, 0.5f);
            Invoke(nameof(CompleteLevel), 1);
        }
    }

    private void CompleteLevel()
    {
        sceneActions.LoadNextScene();
    }
}
