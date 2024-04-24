using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour, IDamageable
{
    private Vector2 inputMovement;
    private Rigidbody2D myRigidBody;
    private Vector2 velocity;
    private CapsuleCollider2D myFeetCollider;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private SpriteRenderer pointer;
    private Light2D light2D;
    private int exitChecks = 0;
    public bool IsAtExit => exitChecks == 2;
    private Status status;
    float damagedShrinkSpeed = 0.2f;

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<CapsuleCollider2D>();
        light2D = GetComponentInChildren<Light2D>();
        pointer.enabled = false;
        PlayerController[] objects = FindObjectsOfType(typeof(PlayerController)) as PlayerController[];
    }

    public void SetStatus(int index)
    {
        if (status == null)
        {
            this.status = new Status(index);
        }
    }

    /// <summary>
    /// Value used to define if a char is bigger, smaller or equal other.
    /// The lower the factor, the bigger the char
    /// </summary>
    public int GetSplitFactor() => status.SplitIndex + status.SplitCount;

    public void Split() => status.Split();

    public void Merge() => status.Merge();

    public bool CanSplit()
    {
        if (status != null)
        {
            return status.CanSplit;
        }
        return false;
    }

    public bool Move()
    {
        if (this == null)
        {
            return false;
        }
        velocity = myRigidBody.velocity;
        velocity.x = inputMovement.x * (moveSpeed * Time.fixedDeltaTime);
        myRigidBody.velocity = velocity;
        return true;
    }

    public void OnMovement(Vector2 value)
    {
        inputMovement = value;
        light2D.enabled = true;
        pointer.enabled = true;
    }

    public Vector2 ShrinkToNewScale(float shrinkSpeed)
    {
        Vector2 newLocalScale = NewSplittedScale(this.transform.localScale);
        this.transform.DOScale(newLocalScale, shrinkSpeed);
        return newLocalScale;
    }

    private Vector2 NewSplittedScale(Vector2 startScale)
    {
        float newX = Mathf.Sqrt(startScale.x * startScale.x / 2);
        float newY = Mathf.Sqrt(startScale.y * startScale.y / 2);
        return new Vector2(newX, newY);
    }

    public void OnJump()
    {
        if (IsOnGround())
        {
            myRigidBody.velocity += (Vector2.up * jumpSpeed);
        }
    }

    public void OnStick()
    {
        if(IsOnWall())
        {
            myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }

    public void OnUnStick()
    {
        myRigidBody.constraints = RigidbodyConstraints2D.None;
    }

    public void StopMovement()
    {
        myRigidBody.velocity = Vector2.zero;
        inputMovement = Vector2.zero;
        light2D.enabled = false;
        pointer.enabled = false;
    }

    public void IncrementExitChecks()
    {
        exitChecks++;
    }

    public void DecrementExitChecks()
    {
        exitChecks--;
    }

    private bool IsFeetTouching(params string[] layers) => myFeetCollider.IsTouchingLayers(LayerMask.GetMask(layers));

    private bool IsOnGround() =>
        IsFeetTouching(Constants.FLOOR_LAYER,
            Constants.PLAYER_LAYER,
            Constants.CHAR_Blue_LAYER,
            Constants.CHAR_Pink_LAYER,
            Constants.CHAR_Green_LAYER,
            Constants.FLOOR_Blue_LAYER,
            Constants.FLOOR_Pink_LAYER,
            Constants.FLOOR_Green_LAYER,
            Constants.Firing_Tower_LAYER);

    private bool IsOnWall() =>
        IsFeetTouching(Constants.Wall_LAYER,
            Constants.Firing_Tower_LAYER);

    public void TakeDamage(int damageAmount)
    {
        while (damageAmount > 0)
        {
            if (!CanSplit())
            {
                this.transform.DOKill();
                Destroy(this.gameObject);
                return;
            }
            Split();
            ShrinkToNewScale(damagedShrinkSpeed);
            damageAmount--;
        }
    }
}
