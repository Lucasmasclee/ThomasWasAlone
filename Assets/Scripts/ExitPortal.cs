using DG.Tweening;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    public delegate void StepOnExitPortal(int value);
    public static event StepOnExitPortal OnStepOnExitPortal;
    private Vector3 initialScale;
    private Vector3 maxScale;
    private Transform spriteTransform;
    private PlayerController myPlayer;
    private Tweener myTweener;

    private void Awake()
    {
        spriteTransform = this.transform.parent.GetComponentInChildren<SpriteRenderer>().transform;
        initialScale = spriteTransform.localScale;
        maxScale = spriteTransform.localScale * 1.1f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision is BoxCollider2D && collision.CompareTag(tag))
        {
            if (!myPlayer)
            {
                myPlayer = collision.GetComponent<PlayerController>();
            }
            myPlayer.IncrementExitChecks();
            if (myPlayer.IsAtExit)
            {
                if (OnStepOnExitPortal != null)
                {
                    OnStepOnExitPortal(1);
                }
                myTweener?.Kill();
                myTweener = spriteTransform.DOPunchScale(maxScale, 2.5f, 3, 0);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision is BoxCollider2D && collision.CompareTag(tag))
        {
            if (!myPlayer)
            {
                myPlayer = collision.GetComponent<PlayerController>();
            }
            if (myPlayer.IsAtExit && OnStepOnExitPortal != null)
            {
                OnStepOnExitPortal(-1);
            }
            myPlayer.DecrementExitChecks();
            myTweener?.Kill();
            spriteTransform.localScale = initialScale;
        }
    }
}
