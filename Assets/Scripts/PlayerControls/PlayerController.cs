using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerController : MonoBehaviour
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

    private void Awake()
    {
        myRigidBody = GetComponent<Rigidbody2D>();
        myFeetCollider = GetComponent<CapsuleCollider2D>();
        light2D = GetComponentInChildren<Light2D>();
        pointer.enabled = false;
    }

    public void Move()
    {
        velocity = myRigidBody.velocity;
        velocity.x = inputMovement.x * (moveSpeed * Time.fixedDeltaTime);
        myRigidBody.velocity = velocity;
    }

    public void OnMovement(Vector2 value)
    {
        inputMovement = value;
        light2D.enabled = true;
        pointer.enabled = true;
    }

    public void OnJump()
    {
        if (IsOnGround())
        {
            myRigidBody.velocity += (Vector2.up * jumpSpeed);
        }
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
            Constants.CHAR_Blue_LAYER,
            Constants.CHAR_Pink_LAYER,
            Constants.CHAR_Green_LAYER,
            Constants.FLOOR_Blue_LAYER,
            Constants.FLOOR_Pink_LAYER,
            Constants.FLOOR_Green_LAYER);
}
