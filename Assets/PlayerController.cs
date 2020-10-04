using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

// ReSharper disable once CheckNamespace
public class PlayerController : MonoBehaviour {
    [SerializeField] private Rigidbody2D myRigidbody2D = null;
    [SerializeField] private Transform feetTransform = null;
    [SerializeField] private Animator animator = null;
    [SerializeField] private LayerMask groundMask = 0;

    [Header("Constants")] [SerializeField] private float runSpeed = 1f;
    [SerializeField] private float baseJumpForce = 400f;
    [Range(0, .3f)] [SerializeField] private float movementSmoothing = .05f;
    // private const float feetRadius = 0.2f;
    private readonly Vector2 feetBox = new Vector2(1.7f, 0.1f);

    /// State Variables
    private Vector2 acceleration = Vector2.zero;

    private float horizontalMove = 0f;
    private bool shouldJump = false;
    private bool onGround = false;
    private float jumpCharge = 0f;
    private bool isCharging = false;
    private bool isFacingLeft = false;
    private bool isFrozen = false;
    private readonly Collider2D[] overlapArray = new Collider2D[5];

    [UsedImplicitly]
    private void Awake() {
        myRigidbody2D = GetComponent<Rigidbody2D>();
    }


    private void Move(float speed, float smoothTime) {
        Vector2 targetVelocity = new Vector2(speed, myRigidbody2D.velocity.y);
        myRigidbody2D.velocity = Vector2.SmoothDamp(
            myRigidbody2D.velocity, targetVelocity, ref acceleration,
            smoothTime, Mathf.Infinity, Time.deltaTime
        );
    }

    private void Move(float speed) {
        Move(speed, movementSmoothing);
    }

    public void SetFreeze(bool freeze) {
        if (freeze) {
            isFrozen = true;
            acceleration = Vector2.zero;
            var velocity = myRigidbody2D.velocity;
            velocity.x = 0;
            myRigidbody2D.velocity = velocity;
            isCharging = false;
            jumpCharge = 0f;
        } else {
            isFrozen = false;
        }
    }

    [UsedImplicitly]
    void Update() {
        if (!isFrozen) {
            horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
            if (Input.GetButtonDown("Jump") && onGround) {
                isCharging = true;
            }

            if (Input.GetButtonUp("Jump") && isCharging) {
                shouldJump = true;
                isCharging = false;
            }

            if (isCharging) {
                jumpCharge += Time.deltaTime;
            }
        } else {
            horizontalMove = 0;
        }


        if ((horizontalMove > 0 && isFacingLeft) || (horizontalMove < 0 && !isFacingLeft)) {
            Flip();
        }
        animator.SetBool("OnGround", onGround);
        animator.SetFloat("SpeedY", myRigidbody2D.velocity.y);
        animator.SetFloat("SpeedX", Mathf.Abs(myRigidbody2D.velocity.x));
        animator.SetFloat("Charge", jumpCharge);
    }

    private void Flip() {
        isFacingLeft = !isFacingLeft;
        GetComponent<SpriteRenderer>().flipX = isFacingLeft;
        // var localScale = transform.localScale;
        // localScale.x *= -1;
        // transform.localScale = localScale;
    }

    [UsedImplicitly]
    void OnDrawGizmos() {
        Gizmos.color = Color.yellow;
        // Gizmos.DrawSphere(feetTransform.position, FeetRadius);
        Gizmos.DrawCube(feetTransform.position, feetBox);
        // var text = $"";
        //
        // var position = transform.position;
        // position.x -= 0.6f;
        // position.y += 1f;
        // Handles.Label(position, text);
    }

    [UsedImplicitly]
    void FixedUpdate() {
        int numberOfHits =
            Physics2D.OverlapBoxNonAlloc(feetTransform.position, feetBox, 0, overlapArray, groundMask);
            // Physics2D.OverlapCircleNonAlloc(feetTransform.position, FeetRadius, overlapArray, groundMask);
        onGround = (numberOfHits > 0);

        if (!isCharging && !isFrozen) {
            Move(horizontalMove);
        } else {
            Move(0, movementSmoothing * 10);
        }

        if (shouldJump && !isFrozen) {
            float jumpForce = baseJumpForce;
            if (jumpCharge > 0.15f) {
                jumpForce *= 1f + Mathf.Clamp(jumpCharge - 0.5f, 0, 0.5f);
            }

            var velocity = myRigidbody2D.velocity;
            velocity.y = 0;
            myRigidbody2D.velocity = velocity;
            myRigidbody2D.AddForce(new Vector2(0f, jumpForce));
            shouldJump = false;
            jumpCharge = 0f;
        }
    }

    [UsedImplicitly]
    void OnTriggerStay2D(Collider2D aCol) {
        if ((groundMask & 1 << aCol.gameObject.layer) == 0) return;
        acceleration.x = 0;
    }
}
