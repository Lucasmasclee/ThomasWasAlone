using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour, IDamageable
{
    private enum StickySide
    {
        LEFT = -1,
        RIGHT = 1
    }
    private Vector2 inputMovement;
    private Rigidbody2D myRigidBody;
    private Vector2 velocity;
    private CapsuleCollider2D myFeetCollider;
    [SerializeField] private float moveSpeed = 100f;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private SpriteRenderer pointer;
    [SerializeField] private float unstickyMagnitude = 3f;
    [SerializeField] private float stickyCooldown = 2f;
    private float realStickyCooldown;
    private bool canSticky;
    private Light2D light2D;
    private Status status;
    private const float damagedShrinkSpeed = 0.2f;
    public bool IsDead { get; private set; } = false;
    private (bool Sticky, StickySide Side) isSticky;
    private const float stickyOffset = 0.02f;

    private void Awake()
    {
        canSticky = true;
        realStickyCooldown = stickyCooldown;
        isSticky = (false, StickySide.LEFT);
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<CapsuleCollider2D>();
        light2D = GetComponentInChildren<Light2D>();
        pointer.enabled = false;
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
        if (status != null && !isSticky.Sticky)
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
        if (!canSticky)
        {
            realStickyCooldown -= Time.fixedDeltaTime;
            canSticky = realStickyCooldown <= 0;
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
        if (isSticky.Sticky)
        {
            UnStick();
        }

        if (!canSticky) return;

        var (hitTopLeft, hitBottonLeft, hitTopRight, hitBottonRight) = TryHitWalls();

        if (hitTopLeft || hitBottonLeft)
        {
            isSticky = (true, StickySide.LEFT);

        }
        else if (hitTopRight || hitBottonRight)
        {
            isSticky = (true, StickySide.RIGHT);
        }
        else
        {
            return;
        }
        myRigidBody.constraints = RigidbodyConstraints2D.FreezeAll;
    }

    private (bool, bool, bool, bool) TryHitWalls()
    {
        Vector2 spriteSize = GetComponent<SpriteRenderer>().bounds.size;
        float distance = spriteSize.y / 2;
        Vector2 top = new Vector2(this.transform.position.x, this.transform.position.y + distance);
        Vector2 botton = new Vector2(this.transform.position.x, this.transform.position.y - distance);
        distance += (GetSplitFactor() * stickyOffset);
        return (HitWall(top, Vector2.left, distance),
            HitWall(botton, Vector2.left, distance),
            HitWall(top, Vector2.right, distance),
            HitWall(botton, Vector2.right, distance));
    }

    private RaycastHit2D HitWall(Vector2 origin, Vector2 direction, float distance)
    {
        Debug.DrawRay(origin, direction, Color.green, 0.4f, false);
        return Physics2D.Raycast(
            origin,
            direction,
            distance,
            LayerMask.GetMask(Constants.Wall_LAYER, Constants.Firing_Tower_LAYER, Constants.FLOOR_LAYER));
    }

    public void UnStick()
    {
        isSticky.Sticky = false;
        myRigidBody.constraints = RigidbodyConstraints2D.FreezeRotation;
        Vector2 direction = isSticky.Side == StickySide.LEFT ? Vector2.right : Vector2.left;
        inputMovement += direction * unstickyMagnitude;
        canSticky = false;
        realStickyCooldown = stickyCooldown;
    }

    public void StopMovement()
    {
        myRigidBody.velocity = Vector2.zero;
        inputMovement = Vector2.zero;
        light2D.enabled = false;
        pointer.enabled = false;
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

    public void TakeDamage(int damageAmount)
    {
        if (isSticky.Sticky)
        {
            UnStick();
        }
        while (damageAmount > 0)
        {
            if (!CanSplit())
            {
                IsDead = true;
                return;
            }
            Split();
            ShrinkToNewScale(damagedShrinkSpeed);
            damageAmount--;
        }
    }
}
